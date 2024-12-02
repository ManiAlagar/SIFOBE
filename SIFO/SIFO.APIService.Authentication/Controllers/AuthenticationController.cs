using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIFO.APIService.Authentication.Service.Contracts;
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

        [HttpPut]
        [Route("ChangePassword")]
        [Authorize(Roles = "Admin,Super Admin")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequest request)
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

        [HttpGet]
        [Route("Page/{id}")]
        public async Task<IActionResult> GetPageByUserIdAsync(long id)
        {
            try
            {
                var result = await _authService.GetPageByUserIdAsync(id);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPut]
        [Route("ForgotPassword")]
        [AllowAnonymous]
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
    }
}
