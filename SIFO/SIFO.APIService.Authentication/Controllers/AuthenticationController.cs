using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIFO.APIService.Authentication.Service.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    { 

        private readonly IAuthenticationService _authService;
        public AuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginAsync(LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPost]
        [Route("Verify-Login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyLoginAsync([FromHeader] long UserId, [FromHeader] long? AuthenticationType, [FromHeader] string? AuthenticationFor , [FromHeader] string? OtpCode)
        {
            try
            {
                var result = await _authService.VerifyLoginAsync(UserId);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPut]
        [Route("ChangePassword")]
        [Authorize(Roles = $"{Constants.ROLE_SUPER_ADMIN},{Constants.ROLE_PP_ADMINISTRATOR},{Constants.ROLE_HOSPITAL_PHARMACY_SUPERVISOR},{Constants.ROLE_HOSPITAL_PHARMACY_OPERATOR},{Constants.ROLE_ADMINISTRATOR},{Constants.ROLE_HOSPITAL_REFERENT},{Constants.ROLE_DOCTOR},{Constants.ROLE_PP_OPERATOR},{Constants.ROLE_RETAIL_PHARMACY_SUPERVISOR},{Constants.ROLE_RETAIL_PHARMACY_OPERATOR}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequest request, [FromHeader] long? UserId, [FromHeader] long? AuthenticationType, [FromHeader] string? AuthenticationFor, [FromHeader] string? OtpCode)
        {
            try
            {
                var result = await _authService.ChangePasswordAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPost]
        [Route("ForgotPassword")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            try
            {
                var result = await _authService.ForgotPasswordAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError($"An error occurred: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpGet("LogOut")]
        public async Task<IActionResult> LogoutAsync()
        {
            try
            {
                var result = await _authService.LogoutAsync();
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError($"An error occurred: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
    }
}
