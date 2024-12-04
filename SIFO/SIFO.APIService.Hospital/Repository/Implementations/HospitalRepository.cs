using SIFO.Model.Entity;
using SIFO.Model.Response;
using SIFO.Model.Constant;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using SIFO.APIService.Hospital.Repository.Contracts;

namespace SIFO.APIService.Hospital.Repository.Implementations
{
    public class HospitalRepository : IHospitalRepository
    {
        private readonly SIFOContext _context;

        public HospitalRepository(SIFOContext context)
        {
            _context = context;
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
    }
}
