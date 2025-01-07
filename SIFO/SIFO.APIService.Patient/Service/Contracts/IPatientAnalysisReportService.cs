using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Patient.Service.Contracts
{
    public interface IPatientAnalysisReportService
    {
        public Task<ApiResponse<PagedResponse<PatientAnalysisReportResponse>>> GetAllPatientAnalysisReportAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, long patientId);
        public Task<ApiResponse<string>> CreatePatientAnalysisReportAsync(PatientAnalysisReportRequest request);
        public Task<ApiResponse<string>> DeletePatientAnalysisReportAsync(long patientAnalysisReportId);
    }
}
