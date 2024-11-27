using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Authentication.Service.Contracts
{
    public interface IAuthenticationService
    {
        public Task<ApiResponse<object>> LoginAsync(LoginRequest request);
        public Task<ApiResponse<LoginResponse>> Login2FAAsync(Login2FARequest request);
        public Task<ApiResponse<string>> ForgotPassword(string email);
        public Task<ApiResponse<string>> ChangePassword(ChangePasswordRequest request);
        public Task<ApiResponse<string>> ForgotPassword(ForgotPasswordRequest request);
    }
}
