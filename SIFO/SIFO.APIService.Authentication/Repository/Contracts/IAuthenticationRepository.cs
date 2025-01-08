using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;


namespace SIFO.APIService.Authentication.Repository.Contracts
{
    public interface IAuthenticationRepository
    {
        public Task<Users> LoginAsync(LoginRequest request); 
        public Task<Users> IsUserExists(long userId);
        public Task<Users> CreateForgotPasswordRequestAsync(ForgotPasswordRequest request);
        public Task<bool> UpdatePasswordAsync(long userId,string hashedPassword, bool isTemp);
        public Task<List<PageResponse>> GetPageByUserIdAsync(long userId);
        public Task<Users> GetUserByEmail(string email);
        public Task<List<RoleResponse>> CreatePermission(long roleId);
        public Task<string> LogoutAsync(long id);
        public Task CreateUserSessionManagementAsync(UserSessionManagement userSessionManagement);
        public Task<List<UserSessionManagement>> GetUserSessionByUserId(long userId);
        public Task<PatientsLoginResponse> LoginAsPatientAsync(LoginRequest request);
       // public Task<Patients> IsPatientExists(long patientId);
    }
}
