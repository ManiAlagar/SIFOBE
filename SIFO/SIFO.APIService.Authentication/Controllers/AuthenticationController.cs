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

        [Authorize(Roles = "Admin")]
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            try
            {
                var result = await _authService.ChangePassword(changePasswordRequest);
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
    }
}
