using FluentValidation;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SIFO.APIService.User.Service.Contracts;

namespace SIFO.APIService.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService; 
        private readonly IValidator<UserRequest> _userValidator;
        public UserController(IUserService userService, IValidator<UserRequest> userValidator)
        {
            _userService = userService;
            _userValidator = userValidator;
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<UserResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUsersAsync([FromHeader] int pageNo = 1, [FromHeader] int pageSize = 10, [FromHeader] string filter = "", [FromHeader] string sortColumn = "Id", [FromHeader] string sortDirection = "DESC", [FromHeader] bool isAll = false)
        {
            try
            {
                var result = await _userService.GetAllUsersAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<PagedResponse<Users>>.InternalServerError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
        
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<UserResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserById(long id)
        {
            try
            {
                var result = await _userService.GetUserById(id);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<PagedResponse<Users>>.InternalServerError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, result);

            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Users>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUserAsync(UserRequest request)
        {
            try
            {
                var validationResult = await _userValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = ApiResponse<List<string>>.BadRequest("Validation Error", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                    return BadRequest(errors);
                }
                var result = await _userService.CreateUserAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Users>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserAsync([FromRoute]long id,[FromBody] UserRequest request, [FromHeader] long UserId, [FromHeader] long? AuthenticationType, [FromHeader] string? AuthenticationFor, [FromHeader] string? OtpCode)
        {
            try
            {
                var validationResult = await _userValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = ApiResponse<List<string>>.BadRequest("Validation Error", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                    return BadRequest(errors);
                } 
                request.UserId= id;
                var result = await _userService.UpdateUserAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<PagedResponse<Users>>.InternalServerError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUserById(long id)
        {
            try
            {
                var result = await _userService.DeleteUserById(id);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<PagedResponse<Users>>.InternalServerError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpGet]
        [Route("userByRoleId/{roleId}")]
        [ProducesResponseType(typeof(ApiResponse<List<UserResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserByRoleIdAsync([FromRoute] long? roleId)
        {
            try
            {
                var result = await _userService.GetUserByRoleId(roleId);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<List<UserResponse>>.InternalServerError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
    }
}
