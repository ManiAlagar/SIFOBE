using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Patient.Service.Contracts
{
    public interface IPatientService
    { 
        public Task<ApiResponse<PagedResponse<PatientResponse>>> GetPatientAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll);
        public Task<ApiResponse<PatientResponse>> GetPatientByIdAsync(long patientId);
        public Task<ApiResponse<string>> CreatePatientAsync(PatientRequest request);
        public Task<ApiResponse<string>> UpdatePatientAsync(PatientRequest request);
        public Task<ApiResponse<string>> DeletePatientAsync(long patientId);
        public Task<ApiResponse<string>> RegisterPatient(RegisterPatientRequest request);
        public Task<ApiResponse<string>> VerifyPatientAsync(VerifyPatientRequest request);
        public Task<ApiResponse<string>> CreatePasswordAsync(CreatePasswordRequest request);
        public Task<ApiResponse<string>> SendOtpAsync(PatientOtpRequest request);
    }
}
