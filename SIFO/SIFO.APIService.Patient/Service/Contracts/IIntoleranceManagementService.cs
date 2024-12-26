using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Patient.Service.Contracts
{
    public interface IIntoleranceManagementService
    {
        public Task<ApiResponse<PagedResponse<IntoleranceManagementResponse>>> GetAllIntoleranceManagementAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll);
        public Task<ApiResponse<IntoleranceManagementResponse>> GetIntoleranceManagementByIdAsync(long intoleranceManagementId);
        public Task<ApiResponse<string>> DeleteIntoleranceManagementAsync(long intoleranceManagementId);
        public Task<ApiResponse<string>> CreateIntoleranceManagementAsync(IntoleranceManagementRequest request);
        public Task<ApiResponse<string>> UpdateIntoleranceManagementAsync(IntoleranceManagementRequest request, long intoleranceManagementId);
    }
}
