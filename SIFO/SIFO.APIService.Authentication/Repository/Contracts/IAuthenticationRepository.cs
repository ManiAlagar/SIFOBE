using SIFO.Model.Entity;
using SIFO.Model.Request;


namespace SIFO.APIService.Authentication.Repository.Contracts
{
    public interface IAuthenticationRepository
    {
        public Task<User> LoginAsync(LoginRequest request); 
        public Task<User> IsUserExists(long userId);
        public Task<bool> ChangePasswordAsync(ChangePasswordRequest request);
        public Task<bool> CreateOtpRequestAsync(OtpRequest otpRequest);
        public Task<OtpRequest> VerifyRequestAsync(Login2FARequest request);
        public Task<OtpRequest> UpdateOtpRequestAsync(Login2FARequest request);
        public Task<User> CreateForgotPasswordRequestAsync(ForgotPasswordRequest request);
        public Task<bool> UpdatePasswordAsync(long userId,string hashedPassword);

    }
}
