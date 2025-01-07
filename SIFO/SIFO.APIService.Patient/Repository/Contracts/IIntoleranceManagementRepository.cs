using SIFO.Model.Entity;
using SIFO.Model.Response;

namespace SIFO.APIService.Patient.Repository.Contracts
{
    public interface IIntoleranceManagementRepository
    {
        public Task<PagedResponse<IntoleranceManagementResponse>> GetAllIntoleranceManagementAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, long patientId);
        public Task<IntoleranceManagementResponse> GetIntoleranceManagementByIdAsync(long intoleranceManagementId);
        public Task<bool> CreateIntoleranceManagementAsync(IntoleranceManagement entity);
        public Task<bool> UpdateIntoleranceManagementAsync(IntoleranceManagement entity, long intoleranceManagementId);
        public Task<string> DeleteIntoleranceManagementAsync(long intoleranceManagementId);
        //public Task<long> IntoleranceManagementDescriptionExistsAsync(string description, long? intoleranceManagementId);
    }
}
