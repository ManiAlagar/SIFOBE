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
    public class HospitalFacilityRepository : IHospitalFacilityRepository
    {
        private readonly SIFOContext _context;
        private readonly ICommonService _commonService;

        public HospitalFacilityRepository(SIFOContext context, ICommonService commonService)
        {
            _context = context;
            _commonService = commonService;
        }

        public async Task<bool> CreateHospitalFacilityAsync(HospitalFacilityRequest request)
        {
            using (var context = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var data = await _commonService.GetDataFromToken();
                    long? addressId = 0;
                    long countryId = await _commonService.GetCountryIdByCountryCodeAsync(request.CountryCode);
                    long? addressDetailResult = await _commonService.AddressDetailExistsAsync(request.Address, request.City, request.Region, countryId, request.ZipCode);

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
                            CountryId = await _commonService.GetCountryIdByCountryCodeAsync(request.CountryCode),
                            Zipcode = request.ZipCode
                        };

                        var addressEntity = await _commonService.CreateAddressDetailAsync(addressDetail);
                        addressId = addressEntity.Id;
                    }

                    var hospitalFacility = new HospitalFacility()
                    {
                        Name = request.HospitalFacilityName,
                        ASL = request.ASL,
                        Province = request.Province,
                        AddressId = addressId.Value,
                        CAP = request.CAP,
                        PhoneNumber = request.PhoneNumber,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = Convert.ToInt64(data.UserId),
                        IsActive = request.IsActive,
                    };

                    var addedData = await _context.HospitalFacilities.AddAsync(hospitalFacility);
                    await _context.SaveChangesAsync();
                    var hospitalFacilityId = hospitalFacility.Id;

                    foreach (var contact in request.Contact)
                    {
                        var contactCreated = await CreateHospitalFacilityContactAsync(contact, hospitalFacilityId);
                        if (!contactCreated)
                        {
                            await _context.Database.RollbackTransactionAsync();
                            return false;
                        }
                    }

                    foreach (var pharmacyId in request.PharmacyIds)
                    {
                        FacilityPharmacyMapping facilityPharmacyMapping = new();
                        facilityPharmacyMapping.PharmacyId = pharmacyId;
                        facilityPharmacyMapping.FacilityId = hospitalFacilityId;
                        var facilityResult = await _context.FacilityPharmacyMappings.AddAsync(facilityPharmacyMapping);
                        if (facilityResult is null)
                        {
                            await _context.Database.RollbackTransactionAsync();
                            return false;
                        }
                    }

                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await _context.Database.RollbackTransactionAsync();
                    return false;
                }
            }
        }

        public async Task<bool> CreateHospitalFacilityContactAsync(ContactRequest contact, long facilityId)
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
                    FacilityId = facilityId,
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

        public async Task<string> DeleteHospitalFacilityAsync(long hospitalFacilityId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var contactResponse = await _context.Contacts.Where(x => x.FacilityId == hospitalFacilityId).ToListAsync();
                    if (contactResponse != null)
                        _context.Contacts.RemoveRange(contactResponse);

                    var response = await _context.FacilityPharmacyMappings.Where(x => x.FacilityId == hospitalFacilityId).ToListAsync();
                    if (response != null)
                        _context.FacilityPharmacyMappings.RemoveRange(response);

                    var hospitalFacilityResponse = await _context.HospitalFacilities.Where(x => x.Id == hospitalFacilityId).SingleOrDefaultAsync();
                    if (hospitalFacilityResponse != null)
                        _context.HospitalFacilities.Remove(hospitalFacilityResponse);

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

        public async Task<PagedResponse<HospitalFacilityResponse>> GetAllHospitalFacilityAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll)
        {
            try
            {
                var query = from hospitalFacilities in _context.HospitalFacilities
                            join addressDetail in _context.AddressDetails on hospitalFacilities.AddressId equals addressDetail.Id
                            join cities in _context.Cities on addressDetail.CityId equals cities.Id
                            join states in _context.States on addressDetail.Region equals states.Id
                            select new HospitalFacilityResponse
                            {
                                Id = hospitalFacilities.Id,
                                Name = hospitalFacilities.Name,
                                ASL = hospitalFacilities.ASL,
                                AddressId = hospitalFacilities.AddressId,
                                CityName = cities.Name,
                                RegionName = states.Name,
                                IsActive = hospitalFacilities.IsActive
                            };

                var count = _context.HospitalFacilities.Count();

                PagedResponse<HospitalFacilityResponse> pagedResponse = new PagedResponse<HospitalFacilityResponse>();

                if (isAll)
                {
                    var result = await query.ToListAsync();
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

        public async Task<HospitalFacilityDetailResponse> GetHospitalFacilityByIdAsync(long hospitalFacilityId)
        {
            try
            {
                var contactsResponse = await _context.Contacts.Where(a => a.FacilityId == hospitalFacilityId).Select(contact =>
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

                var pharmacyIdResponse = await _context.FacilityPharmacyMappings.Where(a => a.FacilityId == hospitalFacilityId).Select(pharmacy =>
                                         pharmacy.PharmacyId).ToListAsync();

                var response = await (from hospitalFacilities in _context.HospitalFacilities
                                      join addressDetail in _context.AddressDetails on hospitalFacilities.AddressId equals addressDetail.Id
                                      join cities in _context.Cities on addressDetail.CityId equals cities.Id
                                      join states in _context.States on addressDetail.Region equals states.Id
                                      where hospitalFacilities.Id == hospitalFacilityId
                                      select new HospitalFacilityDetailResponse
                                      {
                                          Id = hospitalFacilities.Id,
                                          Name = hospitalFacilities.Name,
                                          ASL = hospitalFacilities.ASL,
                                          AddressId = hospitalFacilities.AddressId,
                                          CityName = cities.Name,
                                          RegionName = states.Name,
                                          IsActive = hospitalFacilities.IsActive,
                                          Contacts = contactsResponse,
                                          PharmacyIds = pharmacyIdResponse,
                                      }).FirstOrDefaultAsync(); ;
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateHospitalFacilityAsync(HospitalFacilityRequest request, long hospitalFacilityId)
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
                        CountryId = await _commonService.GetCountryIdByCountryCodeAsync(request.CountryCode),
                        Zipcode = request.ZipCode
                    };

                    var data = await _commonService.GetDataFromToken();
                    var addressData = await _commonService.CreateAddressDetailAsync(addressDetail);

                    var hospitalFacilityData = await _context.HospitalFacilities.Where(a => a.Id == hospitalFacilityId).FirstOrDefaultAsync();
                    if (hospitalFacilityData is not null)
                    {
                        hospitalFacilityData.Name = request.HospitalFacilityName;
                        hospitalFacilityData.AddressId = addressData.Id;
                        hospitalFacilityData.ASL = request.ASL;
                        hospitalFacilityData.Province = request.Province;
                        hospitalFacilityData.PhoneNumber = request.PhoneNumber;
                        hospitalFacilityData.UpdatedBy = Convert.ToInt64(data.UserId);
                        hospitalFacilityData.UpdatedDate = DateTime.UtcNow;
                        hospitalFacilityData.IsActive = request.IsActive;
                        hospitalFacilityData.CAP = request.CAP;

                        _context.HospitalFacilities.Update(hospitalFacilityData);
                        await _context.SaveChangesAsync();

                        var contactData = await _context.Contacts.Where(a => a.FacilityId == hospitalFacilityId).ToListAsync();
                        _context.Contacts.RemoveRange(contactData);

                        foreach (var contact in request.Contact)
                        {
                            var contactCreated = await CreateHospitalFacilityContactAsync(contact, hospitalFacilityId);
                            if (!contactCreated)
                            {
                                await _context.Database.RollbackTransactionAsync();
                                return false;
                            }
                        }

                        var pharmacyData = await _context.FacilityPharmacyMappings.Where(a => a.FacilityId == hospitalFacilityId).ToListAsync();
                        _context.FacilityPharmacyMappings.RemoveRange(pharmacyData);

                        foreach (var pharmacyId in request.PharmacyIds)
                        {
                            FacilityPharmacyMapping facilityPharmacyMapping = new();
                            facilityPharmacyMapping.PharmacyId = pharmacyId;
                            facilityPharmacyMapping.FacilityId = hospitalFacilityId;
                            var facilityResult = await _context.FacilityPharmacyMappings.AddAsync(facilityPharmacyMapping);
                            if (facilityResult is null)
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

        public async Task<List<Pharmacy>> GetPharmaciesByIdsAsync(List<long> request)
        {
            return await _context.Pharmacies.Where(x => request.Contains(x.Id)).ToListAsync();
        }
    }
}
