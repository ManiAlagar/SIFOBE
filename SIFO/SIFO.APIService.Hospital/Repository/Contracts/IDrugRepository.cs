using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Hospital.Repository.Contracts
{
    public interface IDrugRepository
    {
        public Task<bool> SaveDrugAsync(DrugRequest drugRequest);
        public Task<bool> UpdateDrugAsync(DrugRequest drugRequest, long drugId);
        public Task<Drugs?> GetDrugById(long drugId);
        public Task<DrugResponse> GetDrugByIdWithRegionsAsync(long id);
        public Task<bool> DeleteHospitalAsync(Drugs drugs);
       // public Task<PagedResponse<DrugResponse>> GetAllDrugs(int pageNo = 1, int pageSize = 10, string filter = "", string sortColumn = "Id", string sortDirection = "DESC", bool isAll = false);
        public Task<bool> IsRegionExists(long RegionId);
        public Task<bool> IsDrugRegionExists(long? DrugsRegionsId);
        public Task<bool> IsAICExists(string aic, long? drugId = null);
        public Task<bool> IsRegionDuplicated(IEnumerable<DrugRegionRequest> regionIds);

    }
}
