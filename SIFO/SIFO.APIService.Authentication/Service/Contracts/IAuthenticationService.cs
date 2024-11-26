using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Authentication.Service.Contracts
{
    public interface IAuthenticationService
    {
        public Task<ApiResponse<string>> LoginAsync(LoginRequest request);
        public Task<ApiResponse<string>> Login2FAAsync(Login2FARequest request);
        public Task<ApiResponse<string>> ForgotPassword(string email);
        public Task<ApiResponse<string>> ChangePassword(ChangePasswordRequest changePassword);
    }
}
