using SIFO.Model.Entity;
using SIFO.Model.Response;

namespace SIFO.APIService.Master.Repository.Contracts
{
    public interface IStateRepository
    {
        public Task<PagedResponse<StateResponse>> GetAllStateAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll = false);
        public Task<StateResponse> GetStateByIdAsync(long stateId);
        public Task<State> CreateStateAsync(State entity);
        public Task<bool> StateExistsByNameAsync(string stateName);
        public Task<State> UpdateStateAsync(State entity);
        public Task<bool> StateExistsByIdAsync(long? stateId);
        public Task<string> DeleteStateAsync(long stateId);
        public Task<List<StateResponse>> GetStateByCountryIdAsync(string countryCode);
    }
}
