using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Master.Service.Contracts
{
    public interface ICountryService
    {
        public Task<ApiResponse<PagedResponse<CountryResponse>>> GetAllCountryAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll);
        public Task<ApiResponse<CountryResponse>> GetCountryByIdAsync(string countryCode);
        public Task<ApiResponse<Country>> CreateCountryAsync(CountryRequest request);
        public Task<ApiResponse<Country>> UpdateCountryAsync(CountryRequest request);
        public Task<ApiResponse<string>> DeleteCountryAsync(string countryCode);
    }
}
