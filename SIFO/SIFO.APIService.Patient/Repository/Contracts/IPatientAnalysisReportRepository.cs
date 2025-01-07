using SIFO.Model.Entity;
using SIFO.Model.Response;

namespace SIFO.APIService.Patient.Repository.Contracts
{
    public interface IPatientAnalysisReportRepository
    {
        public Task<PagedResponse<PatientAnalysisReportResponse>> GetAllPatientAnalysisReportAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, long patientId);
        public Task<PatientAnalysisReportResponse> GetPatientAnalsisReportByIdAsync(long patientAnalysisReportId);
        public Task<bool> CreatePatientAnalysisReportAsync(PatientAnalysisReport entity);
        public Task<string> DeletePatientAnalysisReportAsync(long patientAnalysisReportId);
    }
}
