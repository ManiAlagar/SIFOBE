using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Authentication.Service.Contracts
{
    public interface IAuthenticationService
    {
        public Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
        public Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest email);
        public Task<ApiResponse<string>> ChangePasswordAsync(ChangePasswordRequest request);
        public Task<ApiResponse<IEnumerable<PageResponse>>> GetPageByUserIdAsync(long userId);
    }
}
