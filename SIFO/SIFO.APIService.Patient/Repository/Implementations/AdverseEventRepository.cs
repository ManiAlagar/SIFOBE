using SIFO.Model.Entity;
using SIFO.Model.Constant;
using SIFO.Model.Response;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using SIFO.APIService.Patient.Repository.Contracts;

namespace SIFO.APIService.Patient.Repository.Implementations
{
    public class AdverseEventRepository : IAdverEventRepository
    {
        private readonly SIFOContext _context;
        public AdverseEventRepository(SIFOContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<AdverseEventResponse>> GetAllAdverseEventAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, long patientId)
        {
            try
            {
                var query = from adverseEvent in _context.AdverseEvent
                            where adverseEvent.PatientId == patientId
                            select new AdverseEventResponse
                            {
                                Id = adverseEvent.Id,
                                Name = adverseEvent.Name,
                                Date = adverseEvent.Date,
                                Intensity = adverseEvent.Intensity,
                                IsActive = adverseEvent.IsActive
                            };

                var count = _context.AdverseEvent.Count();

                PagedResponse<AdverseEventResponse> pagedResponse = new PagedResponse<AdverseEventResponse>();

                if (isAll)
                {
                    var result = await query.ToListAsync();
                    pagedResponse.Result = result;
                    pagedResponse.TotalCount = result.Count;
                    pagedResponse.TotalPages = default;
                    pagedResponse.CurrentPage = default;
                    return pagedResponse;
                }

                string orderByExpression = $"{sortColumn} {sortDirection}";
                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.ToLower();
                    query = query.Where(x => x.Name.ToLower().Contains(filter) || x.Intensity.ToLower().Contains(filter));
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
                throw new Exception($"{ex.Message} - {ex.InnerException}");
            }
        }

        public async Task<AdverseEventResponse> GetAdverseEventByIdAsync(long adverseEventId)
        {
            try
            {
                var response = await (from adverseEvent in _context.AdverseEvent
                                      where adverseEvent.Id == adverseEventId
                                      select new AdverseEventResponse
                                      {
                                          Id = adverseEvent.Id,
                                          Name = adverseEvent.Name,
                                          Date = adverseEvent.Date,
                                          Intensity = adverseEvent.Intensity,
                                          IsActive = adverseEvent.IsActive
                                      }).AsNoTracking().SingleOrDefaultAsync();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} - {ex.InnerException}");
            }
        }

        public async Task<string> CreateAdverseEventAsync(AdverseEvent entity)
        {
            try
            {
                var result = await _context.AdverseEvent.AddAsync(entity);
                await _context.SaveChangesAsync();
                return Constants.SUCCESS;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} - {ex.InnerException}");
            }
        }

        public async Task<string> UpdateAdverseEventAsync(AdverseEvent entity)
        {
            try
            {
                var result = await _context.AdverseEvent.AsNoTracking().Where(a=>a.Id == entity.Id).SingleOrDefaultAsync();
                if (result is null)
                    return Constants.NOT_FOUND; 
                entity.CreatedBy = result.CreatedBy; 
                entity.CreatedDate = result.CreatedDate; 
                _context.AdverseEvent.Update(entity);
                await _context.SaveChangesAsync();
                return Constants.SUCCESS;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} - {ex.InnerException}");
            }
        }

        public async Task<string> DeleteAdverseEventAsync(long adverseEventId)
        {
            try
            {
                var adverseEventData = await _context.AdverseEvent.Where(x => x.Id == adverseEventId).SingleOrDefaultAsync();
                if (adverseEventData is null)
                    return Constants.NOT_FOUND;

                _context.AdverseEvent.Remove(adverseEventData);
                await _context.SaveChangesAsync();
                return Constants.SUCCESS;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} - {ex.InnerException}");
            }
        }

        public async Task<AdverseEvent> AdverseEventNameExistsAsync(string? name,long patientId, long? adverseEventId)
        {
            try
            {
                if (adverseEventId > 0)
                    return await _context.AdverseEvent.Where(c => c.Name.ToLower() == name.Trim().ToLower() && c.Id != adverseEventId && c.PatientId == patientId).FirstOrDefaultAsync();
                else
                    return await _context.AdverseEvent.Where(c => c.Name.ToLower() == name.Trim().ToLower() && c.PatientId == patientId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} - {ex.InnerException}");
            }
        }
    }
}