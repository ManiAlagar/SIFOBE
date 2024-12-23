using SIFO.Model.Entity;
using SIFO.Model.Response;

namespace SIFO.APIService.User.Repository.Contracts
{
    public interface IUserRepository
    {
        public Task<string> CreateUserAsync(Users user,string userId);
        public Task<string> CheckIfEmailOrPhoneExists(string email, string phoneNumber,long? userId = 0);
        public Task<string> DeleteUserById(long id,long roleId,string parentRoleId);
        public Task<UserResponse> GetUserById(long? id, long roleId,string parentRoleId);
        public Task<(string, Users?)> UpdateUserAsync(Users user,string userId);
        public Task<PagedResponse<UserResponse>> GetAllUsersAsync(int pageIndex, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, long? RoleId,string ParentRoleId);
        public Task<(string, bool)> GetPasswordByUserId(long id);
        //public Task<String> InsertUserPharmacyMapping(long userId, long pharmacyId);
    }
}
