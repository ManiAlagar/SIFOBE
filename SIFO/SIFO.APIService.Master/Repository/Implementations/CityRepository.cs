using SIFO.Model.Entity;
using SIFO.Model.Response;
using SIFO.Model.Constant;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using SIFO.APIService.Master.Repository.Contracts;

namespace SIFO.APIService.Master.Repository.Implementations
{
    public class CityRepository : ICityRepository
    {
        private readonly SIFOContext _context;

        public CityRepository(SIFOContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<CityResponse>> GetAllCityAsync(int pageNo = 1, int pageSize = 10, string filter = "", string sortColumn = "Id", string sortDirection = "DESC", bool isAll = false)
        {
            try
            {
                var query = from city in _context.Cities
                            join state in _context.States on city.StateId equals state.Id
                            join country in _context.Countries on city.CountryId equals country.Id
                            select new CityResponse
                            {
                                Id = city.Id,
                                Name = city.Name,
                                StateId = city.StateId,
                                StateCode = city.StateCode,
                                StateName = state.Name,
                                CountryName = country.Name,
                                CountryId = city.CountryId,
                                CountryCode = city.CountryCode,
                                Latitude = city.Latitude,
                                Longitude = city.Longitude,
                                IsActive = city.IsActive
                            };


                var count = _context.Cities.Count();

                PagedResponse<CityResponse> pagedResponse = new PagedResponse<CityResponse>();

                if (isAll)
                {
                    var result = await query.Where(a => a.IsActive).ToListAsync();
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
                    query = query.Where(x => x.Name.ToLower().Contains(filter) || x.CountryCode.ToLower().Contains(filter));
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

        public async Task<CityResponse> GetCityByIdAsync(long cityId)
        {
            try
            {
                var query = from city in _context.Cities
                            join state in _context.States on city.StateId equals state.Id
                            join country in _context.Countries on city.CountryId equals country.Id
                            where city.Id == cityId
                            select new CityResponse
                            {
                                Id = city.Id,
                                Name = city.Name,
                                StateId = city.StateId,
                                StateCode = city.StateCode,
                                StateName = state.Name,
                                CountryName = country.Name,
                                CountryId = city.CountryId,
                                CountryCode = city.CountryCode,
                                Latitude = city.Latitude,
                                Longitude = city.Longitude,
                                IsActive = city.IsActive
                            };

                var result = await query.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<CityResponse>> GetCityByStateIdAsync(long stateId)
        {
            try
            {
                var query = from city in _context.Cities
                            join state in _context.States on city.StateId equals state.Id
                            where state.Id == stateId
                            select new CityResponse
                            {
                                Id = city.Id,
                                Name = city.Name,
                                StateId = city.StateId,
                                StateCode = city.StateCode,
                                StateName = state.Name,
                                CountryId = city.CountryId,
                                CountryCode = city.CountryCode,
                                Latitude = city.Latitude,
                                Longitude = city.Longitude
                            };
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<City> CreateCityAsync(City entity)
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

        public async Task<bool> CityExistsByNameAsync(string cityName)
        {
            return await _context.Cities
                 .Where(c => c.Name.ToLower() == cityName.Trim().ToLower())
                 .AnyAsync();
        }

        public async Task<City> UpdateCityAsync(City entity)
        {
            try
            {
                _context.Cities.Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> CityExistsByIdAsync(long? cityId)
        {
            var res = await _context.Cities.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cityId);
            return res != null;
        }

        public async Task<string> DeleteCityAsync(long cityId)
        {
            try
            {
                var entity = await _context.Cities.Where(x => x.Id == cityId).SingleOrDefaultAsync();
                if (entity != null)
                {
                    _context.Cities.Remove(entity);
                    await _context.SaveChangesAsync();
                    return Constants.SUCCESS;
                }
                return Constants.NOT_FOUND;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

