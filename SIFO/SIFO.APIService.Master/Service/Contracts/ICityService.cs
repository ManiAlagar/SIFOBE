using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Master.Service.Contracts
{
    public interface ICityService
    {
        public Task<ApiResponse<PagedResponse<CityResponse>>> GetAllCityAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll);
        public Task<ApiResponse<CityResponse>> GetCityByIdAsync(long cityId);
        public Task<ApiResponse<City>> CreateCityAsync(CityRequest entity);
        public Task<ApiResponse<City>> UpdateCityAsync(CityRequest entity);
        public Task<ApiResponse<string>> DeleteCityAsync(long cityId);
        public Task<ApiResponse<List<CityResponse>>> GetCityByStateIdAsync(long stateId);
    }
}
