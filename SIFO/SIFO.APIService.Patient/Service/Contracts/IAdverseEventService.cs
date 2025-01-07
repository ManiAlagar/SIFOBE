using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Patient.Service.Contracts
{
    public interface IAdverseEventService
    {
        public Task<ApiResponse<PagedResponse<AdverseEventResponse>>> GetAllAdverseEventAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, long patientId);
        public Task<ApiResponse<AdverseEventResponse>> GetAdverseEventByIdAsync(long adverseEventId);
        public Task<ApiResponse<string>> CreateAdverseEventAsync(AdverseEventRequest request);
        public Task<ApiResponse<string>> UpdateAdverseEventAsync(AdverseEventRequest request);
        public Task<ApiResponse<string>> DeleteAdverseEventAsync(long adverseEventId);
    }
}
