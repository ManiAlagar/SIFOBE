using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Master.Service.Contracts
{
    public interface IMasterService
    {
        public Task<ApiResponse<string>> SendOtpRequestAsync(SendOtpRequest request);
        public Task<ApiResponse<string>> ImportLableAsync(LabelRequest request);
        public Task<ApiResponse<LabelResponse>> GetLabelsAsync();
        public Task<ApiResponse<List<PharmacyTypeResponse>>> GetAllPharmacyTypesAsync();
        public Task<ApiResponse<List<AuthenticationTypeResponse>>> GetAllAuthenticationTypesAsync();
    }
}
