using SIFO.Model.Entity;
using SIFO.Model.Response;

namespace SIFO.APIService.Patient.Repository.Contracts
{
    public interface IAdverEventRepository
    {
        public Task<PagedResponse<AdverseEventResponse>> GetAllAdverseEventAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, long patientId);
        public Task<AdverseEventResponse> GetAdverseEventByIdAsync(long adverseEventId);
        public Task<string> CreateAdverseEventAsync(AdverseEvent entity);
        public Task<string> UpdateAdverseEventAsync(AdverseEvent entity);
        public Task<string> DeleteAdverseEventAsync(long adverseEventId);
        public Task<AdverseEvent> AdverseEventNameExistsAsync(string? name,long patientId, long? adverseEventId);
    }
}
