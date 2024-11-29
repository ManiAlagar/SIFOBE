using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.User.Service.Contracts
{
    public interface IUserService
    {
        public Task<ApiResponse<string>> CreateUserAsync(UserRequest user);
        public Task<ApiResponse<PagedResponse<GetUserResponse>>> GetAllUsersAsync(int pageIndex, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll);
        Task<ApiResponse<string>> DeleteUserById(long id);
        Task<ApiResponse<string>> UpdateUserAsync(UserRequest user);
        Task<ApiResponse<GetUserResponse>> GetUserById(long? id);
    }
}
