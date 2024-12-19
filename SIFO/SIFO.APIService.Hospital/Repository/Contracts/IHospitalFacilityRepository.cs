using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Hospital.Repository.Contracts
{
    public interface IHospitalFacilityRepository
    {
        public Task<PagedResponse<HospitalFacilityResponse>> GetAllHospitalFacilityAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll);
        public Task<HospitalFacilityDetailResponse> GetHospitalFacilityByIdAsync(long hospitalFacilityId);
        public Task<bool> CreateHospitalFacilityAsync(HospitalFacilityRequest request);
        public Task<bool> UpdateHospitalFacilityAsync(HospitalFacilityRequest request, long hospitalFacilityId);
        public Task<string> DeleteHospitalFacilityAsync(long hospitalFacilityId);
        public Task<List<Pharmacy>> GetPharmaciesByIdsAsync(List<long> request);
    }
}
