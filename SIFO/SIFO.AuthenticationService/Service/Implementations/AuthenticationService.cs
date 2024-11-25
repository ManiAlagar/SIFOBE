using SIFO.AuthenticationService.Repository.Contracts;
using SIFO.AuthenticationService.Service.Contracts;
using SIFO.Common.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.AuthenticationService.Service.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {  
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly ICommonService _commonService;
        private readonly JwtTokenGenerator _tokenGenerator;

        public AuthenticationService(IConfiguration configuration,IAuthenticationRepository authenticationRepository, ICommonService commonService, JwtTokenGenerator tokenGenerator)
        {
            _configuration = configuration; 
            _authenticationRepository = authenticationRepository;  
            _commonService = commonService;
            _tokenGenerator = tokenGenerator;
        }
        public async Task<ApiResponse<string>> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return ApiResponse<string>.BadRequest("mail or password cannot be empty");

            var userData = await _authenticationRepository.LoginAsync(request);

            if (userData == null)
                return ApiResponse<string>.UnAuthorized("invalid email and/or password");

            var isFirstLogin = userData.LastLogin == null;
            //var isTempPassword = userData.IsTempPassword == true;
            //if (isFirstLogin || isTempPassword)
            //{
            //    var errorMessage = isFirstLogin ? Constants.FIRST_LOGIN : Constants.USED_TEMP_PASSWORD;
            //    return ApiResponse<string>.Forbidden(errorMessage);
            //} 
            var JwtTokenGenerator = _tokenGenerator.GenerateToken(userData);
            return ApiResponse<string>.Success("succdes", JwtTokenGenerator);
        }

        public async Task<ApiResponse<string>> Login2FAAsync(Login2FARequest request)
        {
            var userContactInfo = await _authenticationRepository.GetUserContactInfo(request.UserId);
            if (userContactInfo == null)
                return ApiResponse<string>.NotFound("user not found");

            if (request.OTPMethod.Trim().ToLower() == Constants.EMAIL.Trim().ToLower())
            {
                string subject = "Your One-Time Password (OTP) for login";
                string body = File.ReadAllText("Templates/EmailLogin.txt").Replace("[UserName]", "").Replace("[OTP Code]", "").Replace("[X]", "");

                string[] mail = new string[] { userContactInfo.Email };
                bool isMailSent = await _commonService.SendMail(mail.ToList(), null, subject, body);
                if (!isMailSent)
                    return ApiResponse<string>.InternalServerError("something went wrong while sending the mail");
                return ApiResponse<string>.Success("successfully sent otp to your mail");
            } 
            else if(request.OTPMethod.Trim().ToLower() == Constants.SMS.Trim().ToLower())
            {
                return ApiResponse<string>.Success();
            }
            else 
                return ApiResponse<string>.BadRequest("unknown request method");
        }

        public async Task<ApiResponse<string>> ForgotPassword(string email)
        {
            throw new NotImplementedException();
        }
    }
}
