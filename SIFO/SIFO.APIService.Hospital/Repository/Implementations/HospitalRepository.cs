using SIFO.Model.Entity;
using SIFO.Model.Response;
using SIFO.Model.Constant;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using SIFO.APIService.Hospital.Repository.Contracts;
using SIFO.Common.Contracts;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using System.Threading.Tasks;
using System;

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

        public async Task<bool> SaveHospitalAsync(HospitalRequest request)
        {
            using (var context = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var hospital = new Model.Entity.Hospital()
                    {
                        Name = request.HospitalName,
                        AddressId = 1,
                        ASL = request.ASL,
                        PhoneNumber = request.PhoneNumber,
                        Province = request.Province,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = 1,
                        IsActive = true,
                        CAB= request.CAB,
                    };

                    var addedData = await _context.Hospitals.AddAsync(hospital);
                    await _context.SaveChangesAsync();
                    var hospitalId = hospital.Id;


                    foreach (var contact in request.Contact)
                    {
                        var contactCreated = await SaveContact(contact, hospitalId);
                        if (!contactCreated)
                        {
                            await _context.Database.RollbackTransactionAsync();
                            return false;
                        }
                    }


                    foreach (var pharmacy in request.Pharmacy)
                    {
                        var pharmacyCreated = await SavePharmacy(pharmacy, hospitalId);
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

        public async Task<bool> SaveContact(ContactRequest contact, long hospitalId)
        {
            try
            {
                //var data= await _commonService.GetDataFromToken();
                var newContact = new Contact()
                {
                    ContactName=contact.ContactName,
                    ContactSurname = contact.ContactSurname,
                    PhoneNumber = contact.PhoneNumber,
                    Role = contact.Role,
                    HospitalId = hospitalId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = 1,
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
        public async Task<bool> SavePharmacy(PharmacyRequest pharmacy, long hospitalId)
        {

            try
            {
                var newPharmacy = new Pharmacy()
                {
                    PharmacyName = pharmacy.PharmacyName,
                    HospitalId = hospitalId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = 1
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
                    var hospitalData = await _context.Hospitals.Where(a => a.Id == id).FirstOrDefaultAsync();

                    if (hospitalData != null)
                    {

                        hospitalData.Name = request.HospitalName;
                        hospitalData.ASL = request.ASL;
                        hospitalData.PhoneNumber = request.PhoneNumber;
                        hospitalData.Province = request.Province;
                        hospitalData.UpdatedDate = DateTime.UtcNow;
                        hospitalData.UpdatedBy = 1;
                        hospitalData.IsActive = request.IsActive;

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
                                    var contactCreated = await SaveContact(contact, id);
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
                                    var pharmacyCreated = await SavePharmacy(pharmacy, id);
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
                                              join pharmacyType in _context.PharmacyTypes on pharmacy.PharmacyTypeId equals pharmacyType.Id
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
                                                  PharmacyTypeId = pharmacy.PharmacyTypeId,
                                                  PharmacyTypes = pharmacyType.Name
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
                                              join pharmacyType in _context.PharmacyTypes on pharmacies.PharmacyTypeId equals pharmacyType.Id
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
                                                  PharmacyTypeId = pharmacies.PharmacyTypeId,
                                                  PharmacyTypes = pharmacyType.Name
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
                if (ex.InnerException is MySqlConnector.MySqlException mysqlEx && mysqlEx.Number == 1451)
                {
                    return Constants.DATADEPENDENCYERRORMESSAGE;
                }
                throw;
            }
        }

    }
}
