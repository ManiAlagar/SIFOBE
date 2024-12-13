using SIFO.Model.Entity;
using SIFO.Model.Response;

namespace SIFO.APIService.User.Repository.Contracts
{
    public interface IUserRepository
    {
        public Task<string> CreateUserAsync(Users user);
        public Task<string> CheckIfEmailOrPhoneExists(string email, string phoneNumber,long? userId = 0);
        public Task<string> DeleteUserById(long id);
        public Task<UserResponse> GetUserById(long? id);
        public Task<Role> GetRoleById(long? id);
        public Task<List<UserResponse>> GetUserByRoleId(long? roleId);
        public Task<string> UpdateUserAsync(Users user,long? existingUserParentId,string parentRoleId);
        public Task<PagedResponse<UserResponse>> GetAllUsersAsync(int pageIndex, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll);
    }
}
