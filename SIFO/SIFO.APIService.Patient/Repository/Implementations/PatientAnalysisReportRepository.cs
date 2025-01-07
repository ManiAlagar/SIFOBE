using SIFO.Model.Entity;
using SIFO.Model.Response;
using SIFO.Model.Constant;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using SIFO.APIService.Patient.Repository.Contracts;

namespace SIFO.APIService.Patient.Repository.Implementations
{
    public class PatientAnalysisReportRepository : IPatientAnalysisReportRepository
    {
        private readonly SIFOContext _context;

        public PatientAnalysisReportRepository(SIFOContext context) 
        {
            _context = context;
        }

        public async Task<PagedResponse<PatientAnalysisReportResponse>> GetAllPatientAnalysisReportAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, long patientId)
        {
            try
            {
                var query = from result in _context.PatientAnalysisReports
                            where result.PatientId == patientId 
                            select new PatientAnalysisReportResponse
                            {
                                Id = result.Id,
                                PatientId = result.PatientId,
                                FilePath = result.FilePath,
                                Attachment_Type = result.Attachment_Type,
                                Description = result.Description,
                                CreatedDate = result.CreatedDate,
                            };

                var count = query.Count();
                PagedResponse<PatientAnalysisReportResponse> pagedResponse = new PagedResponse<PatientAnalysisReportResponse>();

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
                    query = query.Where(x => x.Attachment_Type.ToLower().Contains(filter));
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

        public Task<PatientAnalysisReportResponse> GetPatientAnalsisReportByIdAsync(long patientAnalysisReportId)
        {
            var response = from patientReport in _context.PatientAnalysisReports 
                        where patientReport.Id == patientAnalysisReportId
                        select new PatientAnalysisReportResponse
                        {
                            Id = patientReport.Id,
                            PatientId = patientReport.PatientId,
                            FilePath = patientReport.FilePath,
                            Attachment_Type = patientReport.Attachment_Type,
                            Description = patientReport.Description,
                            CreatedDate = patientReport.CreatedDate,
                        };
            return response.SingleOrDefaultAsync();
        }

        public async Task<bool> CreatePatientAnalysisReportAsync(PatientAnalysisReport entity)
        {
            try
            {
                var result = await _context.PatientAnalysisReports.AddAsync(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> DeletePatientAnalysisReportAsync(long patientAnalysisReportId)
        {
            try
            {
                var entity = await _context.PatientAnalysisReports.Where(x => x.Id == patientAnalysisReportId).SingleOrDefaultAsync();
                _context.PatientAnalysisReports.Remove(entity);
                await _context.SaveChangesAsync();
                return Constants.SUCCESS;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is MySqlConnector.MySqlException mysqlEx && mysqlEx.Number == Constants.DATA_DEPENDENCY_CODE)
                    return Constants.DATA_DEPENDENCY_ERROR_MESSAGE;
                throw;
            }
        }
    }
}
