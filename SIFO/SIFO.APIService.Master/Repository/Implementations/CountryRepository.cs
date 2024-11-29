using SIFO.Model.Entity;
using SIFO.Model.Response;
using SIFO.Model.Constant;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using SIFO.APIService.Master.Repository.Contracts;

namespace SIFO.APIService.Master.Repository.Implementations
{
    public class CountryRepository : ICountryRepository
    {
        private readonly SIFOContext _context;

        public CountryRepository(SIFOContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<CountryResponse>> GetAllCountryAsync(int pageNo = 1, int pageSize = 10, string filter = "", string sortColumn = "Id", string sortDirection = "DESC", bool isAll = false)
        {
            try
            {
                var query = from country in _context.Countries
                            select new CountryResponse
                            {
                                Id = country.Id,
                                Name = country.Name,
                                Iso3 = country.Iso3,
                                Iso2 = country.Iso2,
                                PhoneCode = country.PhoneCode,
                                Timezones = country.Timezones,
                                Latitude = country.Latitude,
                                Longitude = country.Longitude,
                                EmojiU = country.EmojiU
                            };

                var count = (from country in _context.Countries
                             select country).Count();

                PagedResponse<CountryResponse> pagedResponse = new PagedResponse<CountryResponse>();

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
                    query = query.Where(x => x.Name.ToLower().Contains(filter) || x.PhoneCode.ToLower().Contains(filter));
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

        public async Task<CountryResponse> GetCountryByIdAsync(long id)
        {
            try
            {
                var query = from country in _context.Countries
                            where country.Id == id
                            select new CountryResponse
                            {
                                Id = country.Id,
                                Name = country.Name,
                                Iso3 = country.Iso3,
                                Iso2 = country.Iso2,
                                PhoneCode = country.PhoneCode,
                                Timezones = country.Timezones,
                                Latitude = country.Latitude,
                                Longitude = country.Longitude,
                                EmojiU = country.EmojiU
                            };

                var result = await query.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Country> CreateCountryAsync(Country entity)
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

        public async Task<Country> UpdateCountryAsync(Country entity)
        {
            try
            {
                _context.Countries.Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> CountryExistsByNameAsync(string countryName, long? countryId = null)
        {
            return await _context.Countries
                .Where(c => c.Name == countryName && (!countryId.HasValue || c.Id != countryId.Value))
                .AnyAsync();
        }

        public async Task<bool> CountryExistsByIdAsync(long? countryId)
        {
            try
            {
                var res = await _context.Countries.AsNoTracking().FirstOrDefaultAsync(c => c.Id == countryId);
                return res != null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> DeleteCountryAsync(long id)
        {
            try
            {
                var entity = await _context.Countries.Where(x => x.Id == id).SingleOrDefaultAsync();
                if (entity != null)
                {
                    _context.Countries.Remove(entity);
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
