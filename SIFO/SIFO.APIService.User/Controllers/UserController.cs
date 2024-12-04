using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIFO.APIService.User.Service.Contracts;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync(int pageNo = 1, int pageSize = 10, string filter = "", string sortColumn = "Id", string sortDirection = "DESC", bool isAll = false)
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
        [Route("/{id}")]
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
        public async Task<IActionResult> CreateUserAsync(UserRequest user)
        {
            try
            {
                var result = await _userService.CreateUserAsync(user);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserAsync(UserRequest user)
        {
            try
            {
                var result = await _userService.UpdateUserAsync(user);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<PagedResponse<Users>>.InternalServerError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpDelete]
        [Route("/{id}")]
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
    }
}
