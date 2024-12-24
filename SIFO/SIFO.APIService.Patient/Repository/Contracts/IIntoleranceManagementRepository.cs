using SIFO.Model.Entity;
using SIFO.Model.Response;

namespace SIFO.APIService.Patient.Repository.Contracts
{
    public interface IIntoleranceManagementRepository
    {
        public Task<PagedResponse<IntoleranceManagementResponse>> GetAllIntoleranceManagementAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll);
        public Task<IntoleranceManagementResponse> GetIntoleranceManagementByIdAsync(long intoleranceManagementId);
        public Task<string> DeleteIntoleranceManagementAsync(long intoleranceManagementId);
        public Task<bool> CreateIntoleranceManagementAsync(IntoleranceManagement entity);
        public Task<bool> UpdateIntoleranceManagementAsync(IntoleranceManagement request, long intoleranceManagementId);
        public Task<long> IntoleranceManagementNameExistsAsync(string? name, long? allergyId);
    }
}
