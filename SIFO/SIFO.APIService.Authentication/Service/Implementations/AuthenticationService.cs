using SIFO.Common.Contracts;
using SIFO.Core.Repository.Contracts;
using SIFO.Core.Service.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;
using SIFO.APIService.Authentication.Repository.Contracts;
using SIFO.APIService.Authentication.Service.Contracts;

namespace SIFO.APIService.Authentication.Service.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly ICommonService _commonService;
        private readonly JwtTokenGenerator _tokenGenerator;
        private readonly ISendGridService _sendGridService;
        private readonly ITwilioRepository _twilioRepository;

        public AuthenticationService(IConfiguration configuration, IAuthenticationRepository authenticationRepository, ICommonService commonService, JwtTokenGenerator tokenGenerator,
            ISendGridService sendGridService, ITwilioRepository twilioRepository)
        {
            _configuration = configuration;
            _authenticationRepository = authenticationRepository;
            _commonService = commonService;
            _tokenGenerator = tokenGenerator;
            _sendGridService = sendGridService;
            _twilioRepository = twilioRepository;
        }
        public async Task<ApiResponse<object>> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return ApiResponse<object>.BadRequest("mail or password cannot be empty");

            var userData = await _authenticationRepository.LoginAsync(request);

            if (userData == null || userData.PswdUpdatedAt < DateTime.UtcNow.AddMonths(-6))
                return ApiResponse<object>.UnAuthorized("invalid email and/or password");

            var otpResponse = await _commonService.SendOtpRequestAsync(userData.Id, "Login", userData.AuthenticationType.Value);

            if (otpResponse != Constants.SUCCESS)
                return ApiResponse<object>.InternalServerError();

            var response = new
            {
                UserId = userData.Id,
                Sid = await _twilioRepository.GetServiceIdbyUserIDAsync(userData.Id),
            };
            return ApiResponse<object>.Success(otpResponse, response);
        }

        public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return ApiResponse<string>.BadRequest();

            var userData = await _authenticationRepository.GetUserByEmail(request.Email);
            if (userData is null)
                return ApiResponse<string>.NotFound(Constants.NOT_FOUND);

            var password = await _commonService.GenerateRandomPassword(12);
            var passwordHash = await _commonService.EncryptPassword(password);
            
            var isPasswordUpdated = await _authenticationRepository.UpdatePasswordAsync(userData.Id, passwordHash,true);
            if(!isPasswordUpdated) 
                return ApiResponse<string>.InternalServerError();

            var filePath = _configuration["Templates:ForgotPassword"];
            string subject = $"Reset password Request";
            string body = File.ReadAllText(filePath).Replace("[UserName]", $"{userData.FirstName} {userData.LastName}").Replace("[Password]",password);


            //var mailResponse = await _sendGridService.SendMailAsync(request.Email, subject, body, $"{userData.FirstName} {userData.LastName}");  
            var toUser = new string[] { userData.Email };
            var mailResponse = await _commonService.SendMail(toUser.ToList(), null, subject, body); 
            if(!mailResponse)
            //if (!mailResponse.IsSuccess)
                return ApiResponse<string>.InternalServerError("something went wrong while sending the mail");
            return ApiResponse<string>.Success();
        }

        public async Task<ApiResponse<string>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var tokendata = await _commonService.GetDataFromToken();
            request.UserId = Convert.ToInt64(tokendata.UserId);

            var userData = await _authenticationRepository.IsUserExists(request.UserId);

            if (userData == null)
                return ApiResponse<string>.NotFound("user not found");
            
            if(request.OldPassword != userData.PasswordHash)
                return ApiResponse<string>.BadRequest("invalid old password");

            bool isPasswordUpdated = await _authenticationRepository.UpdatePasswordAsync(userData.Id,request.Password ,false);
            if (!isPasswordUpdated)
                return ApiResponse<string>.InternalServerError();
            return ApiResponse<string>.Success();
        }

        public async Task<ApiResponse<LoginResponse>> VerifyLoginAsync(long userId)
        {
            var userData = await _authenticationRepository.IsUserExists(userId);

            if (userData == null)
                return ApiResponse<LoginResponse>.NotFound("user not found");

            var accessToken = _tokenGenerator.GenerateToken(userData);
            LoginResponse loginResponse = new LoginResponse();
            loginResponse.Email = userData.Email;
            loginResponse.UserName = $"{userData.FirstName} {userData.LastName}";
            loginResponse.Token = accessToken;
            loginResponse.RoleId = userData.RoleId;
            loginResponse.RoleName = userData.RoleName ?? string.Empty;
            loginResponse.MenuAccess = await _authenticationRepository.GetPageByUserIdAsync(userData.Id);
            loginResponse.Id = userData.Id;
            loginResponse.IsTempPassword = userData.IsTempPassword == true;
            loginResponse.hasCreatePermission = await _authenticationRepository.CreatePermission(userData.RoleId);

            var userSessionManagement = new UserSessionManagement
            {
                UserId = userData.Id,
                DtLogin = DateTime.UtcNow,
                DtCreation = DateTime.UtcNow,
                DtLogout = null,
                IPAccess = await _commonService.GetIpAddress(),
                TokenSession = accessToken
            };
            await _authenticationRepository.CreateUserSessionManagementAsync(userSessionManagement);

            return ApiResponse<LoginResponse>.Success(Constants.SUCCESS, loginResponse);
        }

        public async Task<ApiResponse<long>> LogoutAsync()
        {
            var userData = await _commonService.GetDataFromToken();
            string response = await _authenticationRepository.LogoutAsync(Convert.ToInt64(userData.UserId));
            
            if (response != Constants.SUCCESS)
                return ApiResponse<long>.InternalServerError();
            return ApiResponse<long>.Success(Constants.SUCCESS);
        }
    }
}
