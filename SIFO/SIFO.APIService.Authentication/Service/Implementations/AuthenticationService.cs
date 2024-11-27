using Microsoft.AspNetCore.Http;
using SIFO.APIService.Authentication.Repository.Contracts;
using SIFO.APIService.Authentication.Service.Contracts;
using SIFO.Common.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Authentication.Service.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly ICommonService _commonService;
        private readonly JwtTokenGenerator _tokenGenerator;

        public AuthenticationService(IConfiguration configuration, IAuthenticationRepository authenticationRepository, ICommonService commonService, JwtTokenGenerator tokenGenerator)
        {
            _configuration = configuration;
            _authenticationRepository = authenticationRepository;
            _commonService = commonService;
            _tokenGenerator = tokenGenerator;
        }
        public async Task<ApiResponse<object>> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return ApiResponse<object>.BadRequest("mail or password cannot be empty");

            var userData = await _authenticationRepository.LoginAsync(request);

            if (userData == null)
                return ApiResponse<object>.UnAuthorized("invalid email and/or password");

            //var isFirstLogin = userData.LastLogin == null;
            //var isTempPassword = userData.IsTempPassword == true;
            //if (isFirstLogin || isTempPassword)
            //{
            //    var errorMessage = isFirstLogin ? Constants.FIRST_LOGIN : Constants.USED_TEMP_PASSWORD;
            //    return ApiResponse<string>.Forbidden(errorMessage);
            //}

            string otp = await _commonService.GenerateOTP(6);
            OtpRequest otpRequest = new OtpRequest();
            otpRequest.CreatedDate = DateTime.UtcNow;
            otpRequest.CreatedBy = userData.Id;
            otpRequest.UserId = userData.Id;
            otpRequest.OtpCode = otp;
            otpRequest.AuthenticatedFor = "Login";
            otpRequest.AuthenticatedType = userData.AuthenticationType;

            otpRequest.ExpirationDate = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["OtpExpiration"]));

            bool isRequestCreated = await _authenticationRepository.CreateOtpRequestAsync(otpRequest);
            if (!isRequestCreated)
                return ApiResponse<object>.InternalServerError("something went wrong while generating otp");

            if (userData.AuthType.ToLower() == Constants.EMAIL)
            {
                string subject = "Your One-Time Password (OTP) for login";
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "../SIFO.Core/Template/EmailLogin.txt");
                string body = File.ReadAllText(filePath).Replace("[UserName]", $"{userData.FirstName} {userData.LastName}").Replace("[OTP Code]", otp).Replace("[X]", _configuration["OtpExpiration"]);

                string[] mail = new string[] { userData.Email };
                bool isMailSent = await _commonService.SendMail(mail.ToList(), null, subject, body);
                if (!isMailSent)
                    return ApiResponse<object>.InternalServerError("something went wrong while sending the mail");
                return ApiResponse<object>.Success("otp has been successfully sent to your email", new { UserId = userData.Id, AuthenticationType = userData.AuthenticationType });
            }
            else if (userData.AuthType.ToLower() == Constants.SMS)
            {
                string body = File.ReadAllText("Templates/SmsLogin.txt").Replace("[UserName]", $"{userData.FirstName} {userData.LastName}").Replace("[OTP Code]", otp).Replace("[X]", "");
                string[] phone = new string[] { userData.PhoneNumber };
                bool isSmsSent = await _commonService.SendSms(phone.ToList(), body);
                if (!isSmsSent)
                    return ApiResponse<object>.InternalServerError("something went wrong while sending the sms");
                return ApiResponse<object>.Success("otp has been successfully sent to your phone number", new { UserId = userData.Id, AuthenticationType = userData.AuthenticationType });
            }
            else if (userData.AuthType.ToLower() == Constants.TWILIO_AUTHY)
            {
                return ApiResponse<object>.Success();
            }
            return ApiResponse<object>.BadRequest("invalid authentication type");
        }

        public async Task<ApiResponse<LoginResponse>> Login2FAAsync(Login2FARequest request)
        {
            request.AuthenticationFor = request.AuthenticationFor.Trim().ToLower();

            var userData = await _authenticationRepository.IsUserExists(request.UserId);
            if (userData == null)
                return ApiResponse<LoginResponse>.NotFound("user not found");

            var otpResponse = await _authenticationRepository.VerifyRequestAsync(request);
            if (otpResponse == null)
                return ApiResponse<LoginResponse>.BadRequest("invalid otp");

            if (otpResponse.ExpirationDate <= DateTime.UtcNow)
                return ApiResponse<LoginResponse>.BadRequest("otp expired");

            var accessToken = await _tokenGenerator.GenerateToken(userData);
            LoginResponse loginResponse = new LoginResponse();
            loginResponse.Email = userData.Email;
            loginResponse.UserName = $"{userData.FirstName} {userData.LastName}";
            loginResponse.Token = accessToken;
            loginResponse.RoleId = userData.RoleId; 
            loginResponse.RoleName = userData.RoleName ?? string.Empty;
            loginResponse.MenuAccess = null;
            loginResponse.Id = userData.Id;

            otpResponse = await _authenticationRepository.UpdateOtpRequestAsync(request);

            return ApiResponse<LoginResponse>.Success("success", loginResponse);
        }

        public async Task<ApiResponse<string>> ForgotPassword(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<string>> ChangePassword(ChangePasswordRequest changePassword)
        {
            var tokendata = await _commonService.GetDataFromToken();
            changePassword.Id = Convert.ToInt64(tokendata.UserId);

            var userData = await _authenticationRepository.IsUserExists(changePassword.Id);

            if (userData == null)
                return ApiResponse<string>.NotFound("user not found");
            
            bool result = await _authenticationRepository.ChangePasswordAsync(changePassword);

            if (result)
                return ApiResponse<string>.Success("password changed successfully");
            else
                return ApiResponse<string>.InternalServerError("something went wrong while changing the password");
        }

        public async Task<ApiResponse<string>> ForgotPassword(ForgotPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email?.Trim()))
                return ApiResponse<string>.BadRequest("Email cannot be null or empty");  

            var userData = await _authenticationRepository.CreateForgotPasswordRequestAsync(request); 
            if(userData == null)
                return ApiResponse<string>.NotFound("user not found"); 

            var password = await _commonService.GenerateRandomPassword(12);
            var hashedPassword = await _commonService.EncryptPassword(password);

            bool isPasswordUpdated = await _authenticationRepository.UpdatePasswordAsync(userData.Id, hashedPassword); 
            if(!isPasswordUpdated)
                return ApiResponse<string>.InternalServerError("something went wrong while updating the password");

            string subject = "Password Reset Request"; 
            string body = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "../SIFO.Core/Template/ForgotPassword.txt")).Replace("[UserName]", $"{userData.FirstName} {userData.LastName}").Replace("[Temporary Password]", password).Replace("[X]", _configuration["OtpExpiration"]);

            var to = new List<string> { request.Email };
            var isMailSent = await _commonService.SendMail(to, null, subject, body);
            if (!isMailSent)
                return ApiResponse<string>.InternalServerError("something went wrong while sending mail");

            return ApiResponse<string>.Success("password reset email sent successfully");
        }
    }
}
