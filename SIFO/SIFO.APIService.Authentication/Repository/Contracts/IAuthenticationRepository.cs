using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;


namespace SIFO.APIService.Authentication.Repository.Contracts
{
    public interface IAuthenticationRepository
    {
        public Task<Users> LoginAsync(LoginRequest request); 
        public Task<Users> IsUserExists(long userId);
        public Task<bool> ChangePasswordAsync(ChangePasswordRequest request);
        public Task<bool> CreateOtpRequestAsync(OtpRequest otpRequest);
        public Task<OtpRequest> VerifyRequestAsync(Login2FARequest request);
        public Task<OtpRequest> UpdateOtpRequestAsync(Login2FARequest request);
        public Task<Users> CreateForgotPasswordRequestAsync(ForgotPasswordRequest request);
        public Task<bool> UpdatePasswordAsync(long userId,string hashedPassword);
        public Task<IEnumerable<PageResponse>> GetPageByUserIdAsync(long userId);
        public Task<Users> GetUserByEmail(string email);
    }
}
