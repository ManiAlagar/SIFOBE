using SIFO.Model.Entity;
using SIFO.Model.Response;

namespace SIFO.APIService.Master.Repository.Contracts
{
    public interface ICountryRepository
    {
        public Task<PagedResponse<CountryResponse>> GetAllCountryAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll = false);
        public Task<CountryResponse> GetCountryByIdAsync(string countryCode);
        public Task<Country> CreateCountryAsync(Country entity);
        public Task<bool> CountryExistsByNameAsync(string countryName);
        public Task<Country> UpdateCountryAsync(Country entity);
        public Task<bool> CountryExistsByIdAsync(string? countryCode);
        public Task<string> DeleteCountryAsync(string countryCode);
    }
}
