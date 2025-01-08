using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Hospital.Service.Contracts
{
    public interface IPharmacyService
    {
        public Task<ApiResponse<PagedResponse<PharmaciesResponse>>> GetPharmacyAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll,string pharmacyType, bool isCurrentUser);
        public Task<ApiResponse<PharmacyDetailResponse>> GetPharmacyByIdAsync(long pharmacyId);
        public Task<ApiResponse<string>> CreatePharmacyAsync(PharmacyRequest request);
        public Task<ApiResponse<string>> UpdatePharmacyAsync(PharmacyRequest request, long pharmacyId);
        public Task<ApiResponse<string>> DeletePharmacyAsync(long pharmacyId);
    }
}
