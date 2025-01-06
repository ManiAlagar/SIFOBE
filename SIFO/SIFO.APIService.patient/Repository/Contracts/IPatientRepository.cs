using SIFO.Model.Response;
using SIFO.Model.Entity;
using SIFO.Model.Request;

namespace SIFO.APIService.Patient.Repository.Contracts
{
    public interface IPatientRepository
    {
        public Task<PagedResponse<PatientResponse>> GetPatientAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll,string roleName);
        public Task<PatientResponse> GetPatientByIdAsync(long patientId);
        public Task<string> CreatePatientAsync(Patients Patient);
        public Task<string> UpdatePatientAsync(Patients Patient);
        public Task<string> DeletePatientByIdAsync(long patientId);
        public Task<string> PhoneNumberOrEmailExists (string phoneNumber,string email,long patientId);
        public Task<string> GetPatientByPhoneNumber(string phoneNumber);
        public Task<bool> AssistedCodeExistsAsync(string assistedCode);
        public Task<Patients> RegisterPatientAsync(Patients entity);
        public Task<long> GetAuthIdByTypeAsync (string authType);
        public Task<OtpRequest> VerifyPatientAsync(VerifyPatientRequest request);
        public Task<string> UpdateOtpDataAsync(OtpRequest request);
        public Task<bool> CreatePasswordRequest(CreatePasswordRequest request, long userId);
        //public Task<string> UpdatePatientAsync(Patient request);
    }
}
