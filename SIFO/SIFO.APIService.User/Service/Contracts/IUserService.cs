using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.User.Service.Contracts
{
    public interface IUserService
    {
        public Task<ApiResponse<string>> CreateUserAsync(UserRequest user);
        public Task<ApiResponse<PagedResponse<UserResponse>>> GetAllUsersAsync(int pageIndex, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, long? RoleId);
        public Task<ApiResponse<string>> DeleteUserById(long id);
        public Task<ApiResponse<string>> UpdateUserAsync(UserRequest user);
        public Task<ApiResponse<UserResponse>> GetUserById(long? id);
        public Task<ApiResponse<List<UserResponse>>> GetUserByRoleId(long? roleId);
    }
}
