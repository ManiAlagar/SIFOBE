using SIFO.Model.Entity;
using SIFO.Model.Response;
using SIFO.Model.Constant;
using SIFO.Model.Request;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using SIFO.APIService.Master.Repository.Contracts;

namespace SIFO.APIService.Master.Repository.Implementations
{
    public class AddressRepository : IAddressRepository
    {
        private readonly SIFOContext _context;

        public AddressRepository(SIFOContext context)
        {
            _context = context;
        }

        public async Task<bool> AddressDetailExistsByIdAsync(long? id)
        {
            var res = await _context.AddressDetails.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            return res != null;
        }

        public async Task<bool> AddressDetailExistsAsync(AddressDetailRequest entity)
        {
            return await _context.AddressDetails
                .Where(c => c.Address.ToLower() == entity.address.Trim().ToLower() || c.CityId == entity.CityId
                 || c.Region == entity.Region || c.CountryId == entity.CountryId || c.Zipcode == entity.Zipcode)
                .AnyAsync();
        }

        public async Task<AddressDetail> CreateAddressDetailAsync(AddressDetail entity)
        {
            try
            {
                var result = await _context.AddAsync(entity);
                await _context.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> DeleteAddressDetailAsync(long id)
        {
            try
            {
                var entity = await _context.AddressDetails.Where(x => x.Id == id).SingleOrDefaultAsync();
                if (entity != null)
                {
                    _context.AddressDetails.Remove(entity);
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

        public async Task<AddressDetailResponse> GetAddressDetailByIdAsync(long id)
        {
            try
            {
                var query = from address in _context.AddressDetails
                            join countries in _context.Countries on address.CountryId equals countries.Id
                            join cities in _context.Cities on address.CityId equals cities.Id
                            where address.Id == id
                            select new AddressDetailResponse
                            {
                                Id = address.Id,
                                Address = address.Address,
                                CityId = address.CityId,
                                CityName = cities.Name,
                                Region = address.Region,
                                CountryId = address.CountryId,
                                CountryName = countries.Name,
                                Zipcode = address.Zipcode,
                                IsActive = address.IsActive,
                                CreatedDate = address.CreatedDate
                            };

                var result = await query.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PagedResponse<AddressDetailResponse>> GetAllAddressDetailAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll = false)
        {
            try
            {
                var query = from address in _context.AddressDetails
                             join countries in _context.Countries on address.CountryId equals countries.Id
                             join cities in _context.Cities on address.CityId equals cities.Id
                             select new AddressDetailResponse
                             {
                                 Id = address.Id,
                                 Address = address.Address,
                                 CityId = address.CityId,
                                 CityName = cities.Name,
                                 Region = address.Region,
                                 CountryId = address.CountryId,
                                 CountryName = countries.Name,
                                 Zipcode = address.Zipcode,
                                 IsActive = address.IsActive,
                                 CreatedDate = address.CreatedDate
                             };

                var count = (from address in _context.AddressDetails
                             select address).Count();

                PagedResponse<AddressDetailResponse> pagedResponse = new PagedResponse<AddressDetailResponse>();

                if (isAll)
                {
                    var result = await query.ToListAsync();
                    pagedResponse.Result = result;
                    pagedResponse.TotalCount = count;
                    pagedResponse.TotalPages = 0;
                    pagedResponse.CurrentPage = 0;
                    return pagedResponse;
                }

                string orderByExpression = $"{sortColumn} {sortDirection}";

                if (filter != null && filter.Length > 0)
                {
                    filter = filter.ToLower();
                    query = query.Where(x => x.Address.ToLower().Contains(filter));
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

        public async Task<AddressDetail> UpdateAddressDetailAsync(AddressDetail entity)
        {
            try
            {
                _context.AddressDetails.Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
