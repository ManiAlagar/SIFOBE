using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;


namespace SIFO.AuthenticationService.Repository.Contracts
{
    public interface IAuthenticationRepository
    {
        public Task<User> LoginAsync(LoginRequest request); 
        public Task<UserResponse> GetUserContactInfo(long userId);
        public Task<bool> ChangePassword(ChangePasswordRequest changePasswordRequest);
    }
}
