using SIFO.Model.Entity;
using SIFO.Model.Response;

namespace SIFO.APIService.Master.Repository.Contracts
{
    public interface ICountryRepository
    {
        public Task<PagedResponse<CountryResponse>> GetAllCountryAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll = false);
        public Task<CountryResponse> GetCountryByIdAsync(long countryId);
        public Task<Country> CreateCountryAsync(Country entity);
        public Task<bool> CountryExistsByNameAsync(string countryName);
        public Task<Country> UpdateCountryAsync(Country entity);
        public Task<bool> CountryExistsByIdAsync(long? countryId);
        public Task<string> DeleteCountryAsync(long countryId);
    }
}
