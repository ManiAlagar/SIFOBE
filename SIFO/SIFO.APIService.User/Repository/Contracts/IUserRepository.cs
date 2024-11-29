using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.User.Repository.Contracts
{
    public interface IUserRepository
    {
        public Task<bool> SaveUserAsync(UserRequest user);
        Task<ApiResponse<string>> CheckIfEmailOrPhoneExists(string email, string phoneNumber,long? userId = 0);
        Task<ApiResponse<string>> DeleteUserById(long id);
        Task<Users> GetUserById(long? id);
        Task<ApiResponse<string>> UpdateUserAsync(UserRequest user);
        Task<ApiResponse<PagedResponse<GetUserResponse>>> GetAllUsersAsync(int pageIndex, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll);
    }
}
