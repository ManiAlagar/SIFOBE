using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Patient.Service.Contracts
{
    public interface IAllergyService
    {
        public Task<ApiResponse<PagedResponse<AllergyResponse>>> GetAllAllergyAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, long patientId);
        public Task<ApiResponse<AllergyResponse>> GetAllergyByIdAsync(long allergyId);
        public Task<ApiResponse<string>> CreateAllergyAsync(AllergyRequest request);
        public Task<ApiResponse<string>> UpdateAllergyAsync(AllergyRequest request, long allergyId);
        public Task<ApiResponse<string>> DeleteAllergyAsync(long allergyId);
    }
}
