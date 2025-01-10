using SIFO.APIService.Authentication.Repository.Contracts;
using SIFO.APIService.Authentication.Service.Contracts;
using SIFO.Common.Contracts;
using SIFO.Core.Repository.Contracts;
using SIFO.Core.Service.Contracts;
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

            request.Password = await _commonService.HashPassword(request.Password);
            var userData = await _authenticationRepository.LoginAsync(request);

            if (userData == null || userData.PswdUpdatedAt < DateTime.UtcNow.AddMonths(-6))
                return ApiResponse<object>.UnAuthorized("invalid email and/or password");

            var userSession = await _authenticationRepository.GetUserSessionByUserId(userData.Id);
            if (userData.IsTempPassword == true && !userSession.Any())
            {
                var loginData = await VerifyLoginAsync(userData.Id); 
                loginData.Data.IsFirstAccess = true;
                return ApiResponse<object>.Success("Success", loginData.Data);
            }
            else if (userData.IsTempPassword == true && userSession.Any())
            {
                var loginData = await VerifyLoginAsync(userData.Id);
                loginData.Data.IsFirstAccess = false;
                return ApiResponse<object>.Success("Success", loginData.Data);
            }
            else
            {
                var otpResponse = await _commonService.SendOtpRequestAsync(userData.Id, "Login", userData.AuthenticationType.Value);
                if (otpResponse != Constants.SUCCESS)
                    return ApiResponse<object>.InternalServerError();
                var response = new
                {
                    UserId = userData.Id,
                    AuthenticationType = userData.AuthenticationType,
                    IsFirstAccess = !userSession.Any(),
                    IsTempPassword = userData.IsTempPassword == true,
                    Sid = await _twilioRepository.GetServiceIdbyUserIDAsync(userData.Id)
                };
                return ApiResponse<object>.Success(otpResponse, response);
            }
        }
        
        public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return ApiResponse<string>.BadRequest();

            var userData = await _authenticationRepository.GetUserByEmail(request.Email);
            if (userData is null)
                return ApiResponse<string>.NotFound(Constants.NOT_FOUND);

            var password = await _commonService.GenerateRandomPassword(12);
            string encryptPassword = await _commonService.EncryptPassword(password);
            string hashedPassword = await _commonService.HashPassword(encryptPassword);

            var isPasswordUpdated = await _authenticationRepository.UpdatePasswordAsync(userData.Id, hashedPassword, true);
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

            request.OldPassword = await _commonService.HashPassword(request.OldPassword);
            if (request.OldPassword != userData.PasswordHash)
                return ApiResponse<string>.BadRequest("invalid old password");

            request.Password = await _commonService.HashPassword(request.Password);
            bool isPasswordUpdated = await _authenticationRepository.UpdatePasswordAsync(userData.Id, request.Password, false);
            if (!isPasswordUpdated)
                return ApiResponse<string>.InternalServerError();
            return ApiResponse<string>.Success();
        }

        public async Task<ApiResponse<LoginResponse>> VerifyLoginAsync(long userId)
        {
            var userData = await _authenticationRepository.IsUserExists(userId);
            if (userData == null)
                return ApiResponse<LoginResponse>.NotFound();

            var accessToken = _tokenGenerator.GenerateToken(userData);
            LoginResponse loginResponse = new LoginResponse();
            loginResponse.Email = userData.Email;
            loginResponse.UserName = $"{userData.FirstName} {userData.LastName}";
            loginResponse.Token = accessToken;
            loginResponse.RoleId = userData.RoleId;
            loginResponse.RoleName = userData.RoleName ?? string.Empty;
            loginResponse.MenuAccess = await _authenticationRepository.GetPageByUserIdAsync(userData.Id);
            loginResponse.UserId = userData.Id;
            loginResponse.IsTempPassword = userData.IsTempPassword == true;
            loginResponse.hasCreatePermission = await _authenticationRepository.CreatePermission(userData.RoleId);
            loginResponse.ParentRoleId = userData.ParentRole.ToList();

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
        public async Task<ApiResponse<PatientsLoginResponse>> LoginAsPatientAsync(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return ApiResponse<PatientsLoginResponse>.BadRequest("Email or password cannot be empty");
            request.Password = await _commonService.HashPassword(request.Password);
            var patientData = await _authenticationRepository.LoginAsPatientAsync(request);

            if (patientData == null || patientData.PswdUpdatedAt < DateTime.UtcNow.AddMonths(-6))
                return ApiResponse<PatientsLoginResponse>.UnAuthorized("Invalid email and/or password");

            var accessToken = _tokenGenerator.GenerateTokenForPatient(patientData);
            var loginResponse = new PatientsLoginResponse
            {
                Email = patientData.Email,
                UserName = $"{patientData.FirstName} {patientData.LastName}",
                Token = accessToken,
                RoleId = patientData.RoleId,
                RoleName = patientData.RoleName ?? string.Empty,
                UserId = patientData.UserId,
                AuthenticationType = patientData.AuthenticationType
            };

            return ApiResponse<PatientsLoginResponse>.Success(Constants.SUCCESS, loginResponse);
        }

       
    }
}
