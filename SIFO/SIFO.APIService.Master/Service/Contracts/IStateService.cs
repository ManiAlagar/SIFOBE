using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Master.Service.Contracts
{
    public interface IStateService
    {
        public Task<ApiResponse<PagedResponse<StateResponse>>> GetAllStateAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll);
        public Task<ApiResponse<StateResponse>> GetStateByIdAsync(long stateId);
        public Task<ApiResponse<State>> CreateStateAsync(StateRequest request);
        public Task<ApiResponse<State>> UpdateStateAsync(StateRequest request);
        public Task<ApiResponse<string>> DeleteStateAsync(long stateId);
        public Task<ApiResponse<List<StateResponse>>> GetStateByCountryIdAsync(long countryId);
    }
}
