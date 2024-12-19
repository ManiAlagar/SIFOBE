using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Hospital.Repository.Contracts
{
    public interface IPharmacyRepository
    {
        public Task<PagedResponse<PharmaciesResponse>> GetAllHospitalPharmacyAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll);
        public Task<PagedResponse<PharmaciesResponse>> GetAllRetailPharmacyAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll);
        public Task<PharmacyDetailResponse> GetPharmacyByIdAsync(long pharmacyId);
        public Task<PharmacyType> GetPharmacyTypeByIdAsync(long pharmacyTypeId);
        public Task<bool> CreatePharmacyAsync(PharmacyRequest request);
        public Task<bool> UpdatePharmacyAsync(PharmacyRequest request, long pharmacyId);
        public Task<string> DeletePharmacyAsync(long pharmacyId);
        public Task<long> GetRetailPharmacyAsync();
        public Task<List<PharmaciesResponse>> GetAllHospitalPharmacyByUserIdAsync(long userId);
        public Task<List<PharmaciesResponse>> GetAllRetailPharmacyByUserIdAsync(long userId);
    }
}
