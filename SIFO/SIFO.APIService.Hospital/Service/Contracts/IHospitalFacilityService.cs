using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Hospital.Service.Contracts
{
    public interface IHospitalFacilityService
    {
        public Task<ApiResponse<PagedResponse<HospitalFacilityResponse>>> GetAllHospitalFacilityAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll);
        public Task<ApiResponse<HospitalFacilityDetailResponse>> GetHospitalFacilityByIdAsync(long hospitalFacilityId);
        public Task<ApiResponse<string>> CreateHospitalFacilityAsync(HospitalFacilityRequest request);
        public Task<ApiResponse<string>> UpdateHospitalFacilityAsync(HospitalFacilityRequest request, long hospitalFacilityId);
        public Task<ApiResponse<string>> DeleteHospitalFacilityAsync(long hospitalFacilityId);
    }
}
