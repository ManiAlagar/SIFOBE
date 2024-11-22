
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIFO.AuthenticationService.Service.Contracts;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthenticationController : ControllerBase
    { 

        private readonly IAuthenticationService _authService;
        public AuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("Login")] 
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
    }
}
