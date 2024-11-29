using SIFO.Model.Entity;
using SIFO.Model.Response;
using SIFO.Model.Constant;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using SIFO.APIService.Master.Repository.Contracts;

namespace SIFO.APIService.Master.Repository.Implementations
{
    public class StateRepository : IStateRepository
    {
        private readonly SIFOContext _context;

        public StateRepository(SIFOContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<StateResponse>> GetAllStateAsync(int pageNo = 1, int pageSize = 10, string filter = "", string sortColumn = "Id", string sortDirection = "DESC", bool isAll = false)
        {
            try
            {
                var query = from state in _context.States
                            join country in _context.Countries on state.CountryId equals Convert.ToInt32(country.Id)
                            select new StateResponse
                            {
                                Id = state.Id,
                                Name = state.Name,
                                CountryId = state.CountryId,
                                CountryCode = state.CountryCode,
                                FipsCode = state.FipsCode,
                                Iso2 = state.Iso2,
                                Type = state.Type,
                                Latitude = state.Latitude,
                                Longitude = state.Longitude,
                                Flag = state.Flag,
                                WikiDataId = state.WikiDataId,
                                CountryName = country.Name
                            };

                var count = (from state in _context.States
                             select state).Count();

                PagedResponse<StateResponse> pagedResponse = new PagedResponse<StateResponse>();

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

        public async Task<StateResponse> GetStateByIdAsync(long id)
        {
            try
            {
                var query = from state in _context.States
                            join country in _context.Countries on state.CountryId equals country.Id
                            where state.Id == id
                            select new StateResponse
                            {
                                Id = state.Id,
                                Name = state.Name,
                                CountryId = state.CountryId,
                                CountryCode = state.CountryCode,
                                FipsCode = state.FipsCode,
                                Iso2 = state.Iso2,
                                Type = state.Type,
                                Latitude = state.Latitude,
                                Longitude = state.Longitude,
                                Flag = state.Flag,
                                WikiDataId = state.WikiDataId,
                                CountryName = country.Name
                            };

                var result = await query.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<StateResponse>> GetStateByCountryIdAsync(long countryId)
        {
            try
            {
                var query = from state in _context.States
                            join country in _context.Countries on state.CountryId equals country.Id
                            where country.Id == countryId
                            select new StateResponse
                            {
                                Id = state.Id,
                                Name = state.Name,
                                CountryId = state.CountryId,
                                CountryCode = state.CountryCode,
                                FipsCode = state.FipsCode,
                                Iso2 = state.Iso2,
                                Type = state.Type,
                                Latitude = state.Latitude,
                                Longitude = state.Longitude,
                                Flag = state.Flag,
                                WikiDataId = state.WikiDataId,
                                CountryName = country.Name
                            };
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<State> CreateStateAsync(State entity)
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

        public async Task<bool> StateExistsByNameAsync(string stateName, long? stateId = null)
        {
            return await _context.States
                .Where(c => c.Name == stateName && (!stateId.HasValue || c.Id != stateId.Value))
                .AnyAsync();
        }

        public async Task<State> UpdateStateAsync(State entity)
        {
            try
            {
                _context.States.Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> StateExistsByIdAsync(long? stateId)
        {
            var res = await _context.States.AsNoTracking().FirstOrDefaultAsync(c => c.Id == stateId);
            return res != null;
        }

        public async Task<string> DeleteStateAsync(long id)
        {
            try
            {
                var entity = await _context.States.Where(x => x.Id == id).SingleOrDefaultAsync();
                if (entity != null)
                {
                    _context.States.Remove(entity);
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
