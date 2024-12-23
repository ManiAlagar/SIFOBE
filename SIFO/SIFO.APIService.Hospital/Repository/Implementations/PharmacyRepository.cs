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
    public class PharmacyRepository : IPharmacyRepository
    {
        private readonly SIFOContext _context;
        private readonly ICommonService _commonService;

        public PharmacyRepository(SIFOContext context, ICommonService commonService)
        {
            _context = context;
            _commonService = commonService;
        }

        public async Task<bool> CreatePharmacyAsync(PharmacyRequest request)
        {
            using (var context = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var data = await _commonService.GetDataFromToken();
                    long addressId = 0;
                    long addressDetailResult = await _commonService.AddressDetailExistsAsync(request.Address, request.City, request.Region, request.CountryId, request.ZipCode);

                    if (addressDetailResult > 0)
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
                            Zipcode = request.ZipCode,
                        };

                        var addressEntity = await _commonService.CreateAddressDetailAsync(addressDetail);
                        addressId = addressEntity.Id;
                    }

                    var pharmacy = new Pharmacy()
                    {
                        //HospitalId = request.HospitalStructureId > 0 ? request.HospitalStructureId : null,
                        AddressId = addressId,
                        PharmacyName = request.Name,
                        PharmacyTypeId = request.PharmacyTypeId,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = Convert.ToInt64(data.UserId),
                        ASL = request.ASL,
                        MinisterialID = request.MinisterialId,
                        IsActive = true,
                        CAP = request.CAP,
                        Province = request.Province,
                        ValidFrom = request.ValidFrom,
                        ValidTo = request.ValidTo,
                    };

                    var addedData = await _context.Pharmacies.AddAsync(pharmacy);
                    await _context.SaveChangesAsync();
                    var pharmacyId = pharmacy.Id;

                    foreach (var contact in request.Contact)
                    {
                        var contactCreated = await CreatePharmacyContactAsync(contact, pharmacyId);
                        if (!contactCreated)
                        {
                            await _context.Database.RollbackTransactionAsync();
                            return false;
                        }
                    }

                    await _context.SaveChangesAsync();
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

        public async Task<bool> CreatePharmacyContactAsync(ContactRequest contact, long pharmacyId)
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
                    PharmacyId = pharmacyId,
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

        public async Task<string> DeletePharmacyAsync(long pharmacyId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var contactResponse = await _context.Contacts.Where(x => x.PharmacyId == pharmacyId).ToListAsync();

                    if (contactResponse != null)
                        _context.Contacts.RemoveRange(contactResponse);

                    var pharmacyResponse = await _context.Pharmacies.Where(x => x.Id == pharmacyId).SingleOrDefaultAsync();
                    if (pharmacyResponse != null)
                        _context.Pharmacies.Remove(pharmacyResponse);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Constants.SUCCESS;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<PharmacyDetailResponse> GetPharmacyByIdAsync(long pharmacyId)
        {
            try
            {
                var contacts = await _context.Contacts.Where(a => a.PharmacyId == pharmacyId).Select(contact =>
                                     new ContactResponse
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
                                     }).ToListAsync();
                var response = await (from pharmacy in _context.Pharmacies
                                      join address in _context.AddressDetails on pharmacy.AddressId equals address.Id
                                      join cities in _context.Cities on address.CityId equals cities.Id
                                      join states in _context.States on address.Region equals states.Id
                                      where pharmacy.Id == pharmacyId
                                      select new PharmacyDetailResponse
                                      {
                                          Id = pharmacy.Id,
                                          Name = pharmacy.PharmacyName,
                                          MinisterialId = pharmacy.MinisterialID,
                                          AddressId = pharmacy.AddressId,
                                          ASL = pharmacy.ASL,
                                          City = address.CityId,
                                          Region = address.Region.Value,
                                          CityName = cities.Name,
                                          RegionName = states.Name,
                                          IsActive = pharmacy.IsActive,
                                          Contacts = contacts,
                                      }).FirstOrDefaultAsync();

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PharmacyType> GetPharmacyTypeByIdAsync(long pharmacyTypeId)
        {
            try
            {
                var response = await _context.PharmacyTypes.Where(m => m.Id == pharmacyTypeId).SingleOrDefaultAsync();
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdatePharmacyAsync(PharmacyRequest request, long pharmacyId)
        {
            using (var context = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    AddressDetail addressDetail = new AddressDetail()
                    {
                        Address = request.Address,
                        CityId = request.City,
                        Region = request.Region,
                        CountryId = request.CountryId,
                        Zipcode = request.ZipCode
                    };

                    var data = await _commonService.GetDataFromToken();
                    var addressData = await _commonService.CreateAddressDetailAsync(addressDetail);

                    var pharmacyData = await _context.Pharmacies.Where(a => a.Id == pharmacyId).FirstOrDefaultAsync();
                    if (pharmacyData != null)
                    {
                        pharmacyData.PharmacyName = request.Name;
                        //pharmacyData.HospitalId = request.HospitalStructureId > 0 ? request.HospitalStructureId : null;
                        pharmacyData.PharmacyTypeId = request.PharmacyTypeId;
                        pharmacyData.UpdatedDate = DateTime.UtcNow;
                        pharmacyData.UpdatedBy = Convert.ToInt64(data.UserId);
                        pharmacyData.ASL = request.ASL;
                        pharmacyData.MinisterialID = request.MinisterialId;
                        pharmacyData.IsActive = request.IsActive;
                        pharmacyData.CAP = request.CAP;
                        pharmacyData.Province = request.Province;
                        pharmacyData.ValidFrom = request.ValidFrom;
                        pharmacyData.ValidTo = request.ValidTo;
                        pharmacyData.AddressId = addressData.Id;

                        _context.Pharmacies.Update(pharmacyData);
                        await _context.SaveChangesAsync();

                        var contactData = await _context.Contacts.Where(a => a.PharmacyId == pharmacyId).ToListAsync();
                        _context.Contacts.RemoveRange(contactData);

                        foreach (var contact in request.Contact)
                        {
                            var contactCreated = await CreatePharmacyContactAsync(contact, pharmacyId);
                            if (!contactCreated)
                            {
                                await _context.Database.RollbackTransactionAsync();
                                return false;
                            }
                        }

                        await _context.SaveChangesAsync();
                        await _context.Database.CommitTransactionAsync();
                        return true;
                    }
                    else
                    {
                        await _context.Database.RollbackTransactionAsync();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    await _context.Database.RollbackTransactionAsync();
                    throw;
                }
            }
        }

        public async Task<long> GetRetailPharmacyAsync()
        {
            try
            {
                var result = await _context.PharmacyTypes.Where(x => x.Name.ToLower().Contains("retail")).Select(x => x.Id).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PagedResponse<PharmaciesResponse>> GetPharmacyAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, string pharmacyType)
        {
            try
            {
                var query = from pharmacy in _context.Pharmacies
                               join address in _context.AddressDetails on pharmacy.AddressId equals address.Id
                               join cities in _context.Cities on address.CityId equals cities.Id
                               join states in _context.States on address.Region equals states.Id
                               where pharmacy.IsActive == true && (string.IsNullOrEmpty(pharmacyType) || (pharmacyType.ToLower().Trim() == "retail" && pharmacy.PharmacyTypeId == GetRetailPharmacyAsync().Result) 
                               || (pharmacyType.ToLower().Trim() == "hospital" && pharmacy.PharmacyTypeId != GetRetailPharmacyAsync().Result))
                               select new PharmaciesResponse
                               {
                                   Id = pharmacy.Id,
                                   Name = pharmacy.PharmacyName,
                                   MinisterialId = pharmacy.MinisterialID,
                                   AddressId = address.Id,
                                   ASL = pharmacy.ASL,
                                   City = address.CityId,
                                   Region = address.Region.Value,
                                   CityName = cities.Name,
                                   RegionName = states.Name,
                                   IsActive = pharmacy.IsActive
                               };

                var count = query.Count();

                PagedResponse<PharmaciesResponse> pagedResponse = new PagedResponse<PharmaciesResponse>();

                if (isAll)
                {
                    var result = await query.Where(a => a.IsActive == true).ToListAsync();
                    pagedResponse.Result = result;
                    pagedResponse.TotalCount = result.Count;
                    pagedResponse.TotalPages = 0;
                    pagedResponse.CurrentPage = 0;
                    return pagedResponse;
                }

                string orderByExpression = $"{sortColumn} {sortDirection}";

                if (filter != null && filter.Length > 0)
                {
                    filter = filter.ToLower();
                    query = query.Where(x => x.Name.ToLower().Contains(filter) || x.CityName.ToLower().Contains(filter) || x.RegionName.ToLower().Contains(filter));
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

        public async Task<List<PharmaciesResponse>> GetMyPharmacyAsync(long userId, string pharmacyType)
        {
            try
            {
                var response = await (from pharmacy in _context.Pharmacies
                                      join userPharmacy in _context.UserPharmacyMappings on pharmacy.Id equals userPharmacy.PharmacyId
                                      join address in _context.AddressDetails on pharmacy.AddressId equals address.Id
                                      join cities in _context.Cities on address.CityId equals cities.Id
                                      join states in _context.States on address.Region equals states.Id
                                      where userId.ToString().Contains(userPharmacy.userId.ToString()) && (string.IsNullOrEmpty(pharmacyType) || (pharmacyType.ToLower().Trim() == "retail" && pharmacy.PharmacyTypeId == GetRetailPharmacyAsync().Result)
                                      || (pharmacyType.ToLower().Trim() == "hospital" && pharmacy.PharmacyTypeId != GetRetailPharmacyAsync().Result))
                                      select new PharmaciesResponse
                                      {
                                          Id = pharmacy.Id,
                                          Name = pharmacy.PharmacyName,
                                          MinisterialId = pharmacy.MinisterialID,
                                          AddressId = address.Id,
                                          ASL = pharmacy.ASL,
                                          City = address.CityId,
                                          Region = address.Region.Value,
                                          CityName = cities.Name,
                                          RegionName = states.Name,
                                          IsActive = pharmacy.IsActive
                                      }).ToListAsync();
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
