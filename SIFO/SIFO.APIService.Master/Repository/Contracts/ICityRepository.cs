using SIFO.Model.Entity;
using SIFO.Model.Response;

namespace SIFO.APIService.Master.Repository.Contracts
{
    public interface ICityRepository
    {
        public Task<PagedResponse<CityResponse>> GetAllCityAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll = false);
        public Task<CityResponse> GetCityByIdAsync(long cityId);
        public Task<City> CreateCityAsync(City entity);
        public Task<bool> CityExistsByNameAsync(string cityName, long? cityId = null);
        public Task<City> UpdateCityAsync(City entity);
        public Task<bool> CityExistsByIdAsync(long? cityId);
        public Task<string> DeleteCityAsync(long cityId);
        public Task<List<CityResponse>> GetCityByStateIdAsync(long stateId);
    }
}
