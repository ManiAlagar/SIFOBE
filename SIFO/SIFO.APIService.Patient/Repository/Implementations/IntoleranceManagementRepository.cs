using SIFO.Model.Entity;
using SIFO.Model.Constant;
using SIFO.Model.Response;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using SIFO.APIService.Patient.Repository.Contracts;

namespace SIFO.APIService.Patient.Repository.Implementations
{
    public class IntoleranceManagementRepository : IIntoleranceManagementRepository
    {
        private readonly SIFOContext _context;

        public IntoleranceManagementRepository(SIFOContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateIntoleranceManagementAsync(IntoleranceManagement entity)
        {
            try
            {
                var result = await _context.IntoleranceManagements.AddAsync(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> DeleteIntoleranceManagementAsync(long intoleranceManagementId)
        {
            try
            {
                var intoleranceManagementResponse = await _context.IntoleranceManagements.Where(x => x.Id == intoleranceManagementId).SingleOrDefaultAsync();
                if (intoleranceManagementResponse is not null)
                {
                    _context.IntoleranceManagements.Remove(intoleranceManagementResponse);
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

        public async Task<PagedResponse<IntoleranceManagementResponse>> GetAllIntoleranceManagementAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll)
        {
            try
            {
                var query = from intoleranceManagement in _context.IntoleranceManagements
                            select new IntoleranceManagementResponse
                            {
                                Id = intoleranceManagement.Id,
                                Name = intoleranceManagement.Name,
                                Description = intoleranceManagement.Description,
                                IsActive = intoleranceManagement.IsActive
                            };

                var count = _context.IntoleranceManagements.Count();

                PagedResponse<IntoleranceManagementResponse> pagedResponse = new PagedResponse<IntoleranceManagementResponse>();

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

        public async Task<IntoleranceManagementResponse> GetIntoleranceManagementByIdAsync(long intoleranceManagementId)
        {
            try
            {
                var response = await (from intoleranceManagement in _context.IntoleranceManagements
                                      where intoleranceManagement.Id == intoleranceManagementId
                                      select new IntoleranceManagementResponse
                                      {
                                          Id = intoleranceManagement.Id,
                                          Name = intoleranceManagement.Name,
                                          Description = intoleranceManagement.Description,
                                          IsActive = intoleranceManagement.IsActive
                                      }).FirstOrDefaultAsync(); ;
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<long> IntoleranceManagementNameExistsAsync(string? name, long? intoleranceManagementId)
        {
            if (intoleranceManagementId > 0)
            {
                return await _context.IntoleranceManagements
                    .Where(c => c.Name.ToLower() == name.Trim().ToLower() && c.Id != intoleranceManagementId).Select(a => a.Id).FirstOrDefaultAsync();
            }
            else
            {
                return await _context.IntoleranceManagements
                     .Where(c => c.Name.ToLower() == name.Trim().ToLower()).Select(a => a.Id).FirstOrDefaultAsync();
            }
        }

        public async Task<bool> UpdateIntoleranceManagementAsync(IntoleranceManagement entity, long intoleranceManagementId)
        {
            try
            {
                var result = await _context.IntoleranceManagements.Where(a => a.Id == intoleranceManagementId).FirstOrDefaultAsync();
                if (result is not null)
                {
                    result.Name = entity.Name;
                    result.Description = entity.Description;
                    result.IsActive = entity.IsActive;
                    result.UpdatedDate = entity.UpdatedDate;
                    result.UpdatedBy = entity.UpdatedBy;    
                    _context.IntoleranceManagements.Update(result);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
