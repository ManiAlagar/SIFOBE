using Microsoft.AspNetCore.Http;
using SIFO.APIService.Authentication.Repository.Contracts;
using SIFO.APIService.Authentication.Service.Contracts;
using SIFO.Common.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography.X509Certificates;

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
        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return ApiResponse<LoginResponse>.BadRequest("mail or password cannot be empty");

            var userData = await _authenticationRepository.LoginAsync(request);

            if (userData == null)
                return ApiResponse<LoginResponse>.UnAuthorized("invalid email and/or password");

            //var isFirstLogin = userData.LastLogin == null;
            //var isTempPassword = userData.IsTempPassword == true;
            //if (isFirstLogin || isTempPassword)
            //{
            //    var errorMessage = isFirstLogin ? Constants.FIRST_LOGIN : Constants.USED_TEMP_PASSWORD;
            //    return ApiResponse<string>.Forbidden(errorMessage);
            //}

            var accessToken = _tokenGenerator.GenerateToken(userData);
            LoginResponse loginResponse = new LoginResponse();
            loginResponse.Email = userData.Email;
            loginResponse.UserName = $"{userData.FirstName} {userData.LastName}";
            loginResponse.Token = accessToken;
            loginResponse.RoleId = userData.RoleId;
            loginResponse.RoleName = userData.RoleName ?? string.Empty;
            loginResponse.MenuAccess = null;
            loginResponse.Id = userData.Id;

            return ApiResponse<LoginResponse>.Success("success", loginResponse);
        }

        public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return ApiResponse<string>.BadRequest();

            var userData = await _authenticationRepository.GetUserByEmail(request.Email);
            if (userData is null)
                return ApiResponse<string>.BadRequest(Constants.NOT_FOUND);

            var password = await _commonService.GenerateRandomPassword(12);
            var passwordHash = await _commonService.EncryptPassword(password);
            
            var isPasswordUpdated = await _authenticationRepository.UpdatePasswordAsync(userData.Id, passwordHash);
            if(!isPasswordUpdated) 
                return ApiResponse<string>.InternalServerError();

            var filePath = _configuration["Templates:ForgotPassword"];
            string subject = $"Reset password Request";
            string body = File.ReadAllText(filePath).Replace("[UserName]", $"{userData.FirstName} {userData.LastName}").Replace("[Password]",password);
            
            string[] mail = new string[] { userData.Email };
            bool isMailSent = await _commonService.SendMail(mail.ToList(), null, subject, body);
            if (!isMailSent)
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

            bool isPasswordUpdated = await _authenticationRepository.UpdatePasswordAsync(userData.Id,request.Password);
            if (!isPasswordUpdated)
                return ApiResponse<string>.InternalServerError();
            return ApiResponse<string>.Success();
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

        public async Task<ApiResponse<IEnumerable<PageResponse>>> GetPageByUserIdAsync(long id)
        {
            var response = await _authenticationRepository.GetPageByUserIdAsync(id);
            return ApiResponse<IEnumerable<PageResponse>>.Success("success",response);
        }
    }
}
