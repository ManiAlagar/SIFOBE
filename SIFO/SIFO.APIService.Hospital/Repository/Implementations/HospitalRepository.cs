using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;
using SIFO.Model.Constant;
using SIFO.Common.Contracts;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using SIFO.APIService.Hospital.Repository.Contracts;

namespace SIFO.APIService.Hospital.Repository.Implementations
{
    public class HospitalRepository : IHospitalRepository
    {
        private readonly SIFOContext _context;
        private readonly ICommonService _commonService;

        public HospitalRepository(SIFOContext context, ICommonService commonService)
        {
            _context = context;
            _commonService = commonService;
        }

        public async Task<bool> CreateHospitalAsync(HospitalRequest request)
        {
            using (var context = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var data = await _commonService.GetDataFromToken();
                    long? addressId = 0;
                    long? addressDetailResult = await _commonService.AddressDetailExistsAsync(request.Address, request.City, request.Region, request.CountryId, request.ZipCode);

                    if (addressDetailResult > 0 && addressDetailResult is not null)
                    {
                        addressId = addressDetailResult;
                    }
                    else
                    {
                        AddressDetail addressDetail = new AddressDetail()
                        {
                            Address = request.Address,
                            CityId = request.City,
                            Region = request.Region,
                            CountryId = request.CountryId,
                            Zipcode = request.ZipCode
                        };

                        var addressEntity = await _commonService.CreateAddressDetailAsync(addressDetail);
                        addressId = addressEntity.Id;
                    }

                    var hospital = new Model.Entity.Hospital()
                    {
                        Name = request.HospitalName,
                        AddressId = addressId,
                        ASL = request.ASL,
                        PhoneNumber = request.PhoneNumber,
                        Province = request.Province,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = Convert.ToInt64(data.UserId),
                        IsActive = true,
                        CAB = request.CAB,
                    };

                    var addedData = await _context.Hospitals.AddAsync(hospital);
                    await _context.SaveChangesAsync();
                    var hospitalId = hospital.Id;

                    foreach (var contact in request.Contact)
                    {
                        var contactCreated = await CreateContactAsync(contact, hospitalId);
                        if (!contactCreated)
                        {
                            await _context.Database.RollbackTransactionAsync();
                            return false;
                        }
                    }

                    foreach (var pharmacy in request.Pharmacy)
                    {
                        var pharmacyCreated = await CreatePharmacyAsync(pharmacy, hospitalId);
                        if (!pharmacyCreated)
                        {
                            await _context.Database.RollbackTransactionAsync();
                            return false;
                        }
                    }

                    await _context.Database.CommitTransactionAsync();
                    return true;
                }
                catch (Exception e)
                {
                    await _context.Database.RollbackTransactionAsync();
                    return false;
                }
            }
        }


        public async Task<bool> CheckIfEmailOrPhoneExists(string phoneNumber, long userId)
        {
            return await _context.Hospitals.AnyAsync(a => a.PhoneNumber == phoneNumber);

        }

        public async Task<bool> CreateContactAsync(ContactRequest contact, long hospitalId)
        {
            try
            {
                var data = await _commonService.GetDataFromToken();
                var newContact = new Contact()
                {
                    ContactName = contact.ContactName,
                    ContactSurname = contact.ContactSurname,
                    PhoneNumber = contact.PhoneNumber,
                    Role = contact.Role,
                    HospitalId = hospitalId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = Convert.ToInt64(data.UserId),
                };
                await _context.Contacts.AddAsync(newContact);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreatePharmacyAsync(PharmacyRequest pharmacy, long hospitalId)
        {
            try
            {
                var data = await _commonService.GetDataFromToken();

                var newPharmacy = new Pharmacy()
                {
                    PharmacyName = pharmacy.PharmacyName,
                    HospitalId = hospitalId, 
                    PharmacyType = pharmacy.PharmacyType, 
                    ValidFrom = pharmacy.ValidFrom,
                    ValidTo = pharmacy.ValidTo,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = Convert.ToInt64(data.UserId),
                };

                await _context.Pharmacies.AddAsync(newPharmacy);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> UpdateHospitalAsync(HospitalRequest request, long id)
        {
            using (var context = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    AddressDetail addressDetail = new AddressDetail()
                    { 
                        Id = request.AddressId,
                        Address = request.Address,
                        CityId = request.City,
                        Region = request.Region,
                        CountryId = request.CountryId,
                        Zipcode = request.ZipCode
                    };
                    var data = await _commonService.GetDataFromToken();
                    var addressData = await _commonService.UpdateAddressDetailAsync(addressDetail);
                    if (addressData is null)
                        return false;

                    var hospitalData = await _context.Hospitals.Where(a => a.Id == id).FirstOrDefaultAsync();
                    if (hospitalData != null)
                    {
                        hospitalData.Name = request.HospitalName;
                        hospitalData.ASL = request.ASL;
                        hospitalData.PhoneNumber = request.PhoneNumber;
                        hospitalData.Province = request.Province;
                        hospitalData.UpdatedDate = DateTime.UtcNow;
                        hospitalData.UpdatedBy = Convert.ToInt64(data.UserId);
                        hospitalData.IsActive = request.IsActive;
                        hospitalData.AddressId = addressData.Id;
                        hospitalData.CAB = request.CAB;

                        _context.Hospitals.Update(hospitalData);
                        await _context.SaveChangesAsync();
                        if (request.IsActive)
                        {
                            foreach (var contact in request.Contact)
                            {
                                if (contact.IsDeleted == true)
                                {
                                    var contactData = await _context.Contacts.Where(a => a.Id == contact.Id
                                      && a.HospitalId == id).FirstOrDefaultAsync();

                                    if (contactData != null)
                                        _context.Contacts.Remove(contactData);
                                }

                                if (contact.IsNew == true)
                                {
                                    var contactCreated = await CreateContactAsync(contact, id);
                                }
                            }

                            foreach (var pharmacy in request.Pharmacy)
                            {
                                if (pharmacy.IsDeleted == true)
                                {
                                    var pharmacyData = await _context.Pharmacies.Where(a => a.Id == pharmacy.Id
                                    && a.HospitalId == id).FirstOrDefaultAsync();

                                    if (pharmacyData != null)
                                        _context.Pharmacies.Remove(pharmacyData);

                                }

                                if (pharmacy.IsNew == true)
                                {
                                    var pharmacyCreated = await CreatePharmacyAsync(pharmacy, id);
                                }
                            }
                            await _context.SaveChangesAsync();
                            await _context.Database.CommitTransactionAsync();
                        }
                        return true;
                    }
                    else
                    {
                        await _context.Database.RollbackTransactionAsync();
                        return false;
                    }
                }
                catch (Exception e)
                {
                    await _context.Database.RollbackTransactionAsync();
                    return false;
                }
            }
        }
        public async Task<HospitalResponse> GetHospitalByIdAsync(long hospitalId)
        {
            try
            {
                var query = from hospital in _context.Hospitals
                            join contact in _context.Contacts on hospital.Id equals contact.HospitalId
                            join pharmacy in _context.Pharmacies on hospital.Id equals pharmacy.HospitalId
                            where hospital.Id == hospitalId
                            select new HospitalResponse
                            {
                                Id = hospital.Id,
                                Name = hospital.Name,
                                AddressId = hospital.AddressId,
                                ASL = hospital.ASL,
                                Province = hospital.Province,
                                PhoneNumber = hospital.PhoneNumber,
                                CreatedDate = hospital.CreatedDate,
                                CreatedBy = hospital.CreatedBy,
                                UpdatedDate = hospital.UpdatedDate,
                                UpdatedBy = hospital.UpdatedBy,
                                IsActive = hospital.IsActive,
                                Contacts = (from contact in _context.Contacts
                                            where contact.HospitalId == hospital.Id
                                            select new ContactResponse
                                            {
                                                Id = contact.Id,
                                                ContactName = contact.ContactName,
                                                ContactSurname = contact.ContactSurname,
                                                Role = contact.Role,
                                                PhoneNumber = contact.PhoneNumber,
                                                CreatedDate = contact.CreatedDate,
                                                CreatedBy = contact.CreatedBy,
                                                UpdatedDate = contact.UpdatedDate,
                                                UpdatedBy = contact.UpdatedBy,
                                                IsActive = contact.IsActive,
                                            }).ToList(),
                                Pharmacies = (from pharmacy in _context.Pharmacies
                                              where pharmacy.HospitalId == hospital.Id
                                              select new PharmacyResponse
                                              {
                                                  Id = pharmacy.Id,
                                                  PharmacyName = pharmacy.PharmacyName,
                                                  CreatedDate = pharmacy.CreatedDate,
                                                  CreatedBy = pharmacy.CreatedBy,
                                                  UpdatedDate = pharmacy.UpdatedDate,
                                                  UpdatedBy = pharmacy.UpdatedBy,
                                                  IsActive = pharmacy.IsActive,
                                                  PharmacyTypes = pharmacy.PharmacyType,
                                                  ValidFrom = pharmacy.ValidFrom, 
                                                  ValidTo = pharmacy.ValidTo,
                                              }).ToList(),
                            };

                var result = await query.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PagedResponse<HospitalResponse>> GetAllHospitalAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection)
        {
            try
            {
                var query = from hospital in _context.Hospitals
                            join contact in _context.Contacts on hospital.Id equals contact.HospitalId
                            join pharmacy in _context.Pharmacies on hospital.Id equals pharmacy.HospitalId
                            select new HospitalResponse
                            {
                                Id = hospital.Id,
                                Name = hospital.Name,
                                AddressId = hospital.AddressId,
                                ASL = hospital.ASL,
                                Province = hospital.Province,
                                PhoneNumber = hospital.PhoneNumber,
                                CreatedDate = hospital.CreatedDate,
                                CreatedBy = hospital.CreatedBy,
                                UpdatedDate = hospital.UpdatedDate,
                                UpdatedBy = hospital.UpdatedBy,
                                IsActive = hospital.IsActive,
                                Contacts = (from contact in _context.Contacts
                                            where contact.HospitalId == hospital.Id && contact.IsActive == true
                                            select new ContactResponse
                                            {
                                                Id = contact.Id,
                                                ContactName = contact.ContactName,
                                                ContactSurname = contact.ContactSurname,
                                                Role = contact.Role,
                                                PhoneNumber = contact.PhoneNumber,
                                                CreatedDate = contact.CreatedDate,
                                                CreatedBy = contact.CreatedBy,
                                                UpdatedDate = contact.UpdatedDate,
                                                UpdatedBy = contact.UpdatedBy,
                                                IsActive = contact.IsActive,
                                            }).ToList(),
                                Pharmacies = (from pharmacies in _context.Pharmacies
                                              where pharmacies.HospitalId == hospital.Id && pharmacies.IsActive == true
                                              select new PharmacyResponse
                                              {
                                                  Id = pharmacies.Id,
                                                  PharmacyName = pharmacies.PharmacyName,
                                                  CreatedDate = pharmacies.CreatedDate,
                                                  CreatedBy = pharmacies.CreatedBy,
                                                  UpdatedDate = pharmacies.UpdatedDate,
                                                  UpdatedBy = pharmacies.UpdatedBy,
                                                  IsActive = pharmacies.IsActive,
                                                  PharmacyTypes = pharmacies.PharmacyType,
                                                  ValidFrom = pharmacy.ValidFrom,
                                                  ValidTo = pharmacy.ValidTo,
                                              }).ToList()
                            };

                var count = (from hospital in _context.Hospitals
                             select hospital).Count();

                PagedResponse<HospitalResponse> pagedResponse = new PagedResponse<HospitalResponse>();

                string orderByExpression = $"{sortColumn} {sortDirection}";

                if (filter != null && filter.Length > 0)
                {
                    filter = filter.ToLower();
                    query = query.Where(x => x.Name.ToLower().Contains(filter));
                    count = query.Count();
                }
                query = query.OrderBy(orderByExpression).Skip((pageNo - 1) * pageSize).Take(pageSize).AsQueryable();

                pagedResponse.Result = query;
                pagedResponse.TotalCount = count;
                pagedResponse.TotalPages = (int)Math.Ceiling((pagedResponse.TotalCount ?? 0) / (double)pageSize);
                pagedResponse.CurrentPage = pageNo;
                return pagedResponse;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> DeleteHospitalAsync(long hospitalId)
        {
            try
            {
                var entity = await _context.Hospitals.Where(x => x.Id == hospitalId).SingleOrDefaultAsync();
                if (entity != null)
                {
                    _context.Hospitals.Remove(entity);
                    await _context.SaveChangesAsync();
                    return Constants.SUCCESS;
                }
                return Constants.NOT_FOUND;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is MySqlConnector.MySqlException mysqlEx && mysqlEx.Number == Constants.DATADEPENDENCYCODE)
                {
                    return Constants.DATADEPENDENCYERRORMESSAGE;
                }
                throw;
            }
        }
        public async Task<Dictionary<string, List<CalendarResponse>>> GetCalendarByIdAsync(long pharmacyId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var calendarResult = await _context.Calendar
                .Where(a => a.PharmacyId == pharmacyId && a.CalendarDate.Value.Date >= startDate.Date && a.CalendarDate.Value.Date <= endDate.Date)   
                .Select(a => new CalendarResponse
                {
                    OpeningTime = a.OpeningTime,
                    ClosingTime = a.ClosingTime,
                    CalendarDate = a.CalendarDate,
                    IsHoliday = a.IsHoliday,
                    PharmacyId = a.PharmacyId,
                })
                .ToListAsync();

                var defaultCalendarData = await _context.Calendar.Where(a => a.PharmacyId == null && a.CalendarDate == null).Select(a => new CalendarResponse
                {
                    OpeningTime = a.OpeningTime,
                    ClosingTime = a.ClosingTime,
                    CalendarDate = a.CalendarDate,
                    IsHoliday = a.IsHoliday,
                    PharmacyId = a.PharmacyId,
                }).FirstOrDefaultAsync();

                var dateRange = Enumerable.Range(0, (endDate.Day - startDate.Day) + 1)
                                      .Select(i => startDate.AddDays(i))
                                      .ToList();

                Dictionary<string, List<CalendarResponse>> response = new();

                foreach (var date in dateRange)
                {
                    string key = date.ToString("dd-MM-yyyy");

                    if (calendarResult.Any(a => a.CalendarDate.Value.ToString("dd-MM-yyyy") == key))
                    {
                        response[key] = calendarResult.Where(a => a.CalendarDate.Value.ToString("dd-MM-yyyy") == key).ToList();
                    }
                    else
                    {
                        List<CalendarResponse> calResponse = new();
                        CalendarResponse calendarResponse = new CalendarResponse();
                        calendarResponse.IsHoliday = defaultCalendarData.IsHoliday;
                        calendarResponse.CalendarDate = Convert.ToDateTime(key);
                        calendarResponse.PharmacyId = pharmacyId;
                        calendarResponse.OpeningTime = defaultCalendarData.OpeningTime;
                        calendarResponse.ClosingTime = defaultCalendarData.ClosingTime;
                        calResponse.Add(calendarResponse);
                        response[key] = calResponse;
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> CreateCalendarAsync(Calendar request)
        {
            try
            {
                var entity = await _context.Calendar.AddAsync(request);
                await _context.SaveChangesAsync(); 
                return Constants.SUCCESS;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> UpdateCalendarAsync(Calendar request)
        {
            try
            {
                _context.Calendar.Update(request);
                await _context.SaveChangesAsync();
                return Constants.SUCCESS;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> CalendarExistsAsync(long id)
        {
            try
            {
                var calendarData = await _context.Calendar.AsNoTracking().Where(a => a.Id == id).SingleOrDefaultAsync();
                return calendarData != null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> GetPharmacyByIdAsync(long id)
        {
            try
            {
                var result = await _context.Pharmacies.Where(a => a.Id == id).SingleOrDefaultAsync(); 
                return result != null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
