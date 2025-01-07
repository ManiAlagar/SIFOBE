using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Hospital.Service.Contracts
{
    public interface IDrugService
    {
        public Task<ApiResponse<string>> CreateDrugAsync(DrugRequest drugRequest);
        public Task<ApiResponse<string>> UpdateDrugAsync(DrugRequest drugRequest, long drugId);
        public Task<ApiResponse<DrugResponse>> GetDrugByIdWithRegionsAsync(long id);
        public Task<ApiResponse<string>> DeleteHospitalAsync(long id);
      //  public Task<ApiResponse<PagedResponse<DrugResponse>>> GetAllDrugs(int pageNo = 1, int pageSize = 10, string filter = "", string sortColumn = "Id", string sortDirection = "DESC", bool isAll = false);
    }
}
