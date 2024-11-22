using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.AuthenticationService.Service.Contracts
{
    public interface IAuthenticationService
    {
        public Task<ApiResponse<string>> LoginAsync(LoginRequest request);
        public Task<ApiResponse<string>> Login2FAAsync(Login2FARequest request); 
        public Task<ApiResponse<string>> ForgotPassword(string email);

    }
}
