using FluentValidation;
using SIFO.Model.Request;
using SIFO.Model.Response;
using Microsoft.AspNetCore.Mvc;
using SIFO.APIService.Hospital.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using SIFO.Model.Entity;

namespace SIFO.APIService.Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HospitalController : ControllerBase
    {
        private readonly IHospitalService _hospitalService;
        private readonly IValidator<HospitalRequest> _hospitalValidator;
        public HospitalController(IHospitalService hospitalService, IValidator<HospitalRequest> hospitalValidator)
        {
            _hospitalService = hospitalService;
            _hospitalValidator = hospitalValidator;
        }

        [HttpPost]
        [Route("Hospital")]
        [ProducesResponseType(typeof(ApiResponse<Model.Entity.Hospital>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateHospitalAsync(HospitalRequest request)
        {
            try
            {
                var validationResult = await _hospitalValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = ApiResponse<List<string>>.BadRequest("Validation Error", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                    return BadRequest(errors);
                }
                var result = await _hospitalService.CreateHospitalAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpGet]
        [Route("Hospital/{hospitalId}")]
        [ProducesResponseType(typeof(ApiResponse<HospitalResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHospitalByIdAsync([FromRoute] long hospitalId)
        {
            try
            {
                var result = await _hospitalService.GetHospitalByIdAsync(hospitalId);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpDelete]
        [Route("Hospital/{hospitalId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHospitalAsync([FromRoute] long hospitalId)
        {
            try
            {
                var result = await _hospitalService.DeleteHospitalAsync(hospitalId);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpGet]
        [Route("Hospital")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<HospitalResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllHospitalAsync([FromHeader] int pageNo = 1, [FromHeader] int pageSize = 10, [FromHeader] string filter = "", [FromHeader] string sortColumn = "Id", [FromHeader] string sortDirection = "DESC")
        {
            try
            {
                var result = await _hospitalService.GetAllHospitalAsync(pageNo, pageSize, filter, sortColumn, sortDirection);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPut]
        [Route("{HospitalId}")]
        [ProducesResponseType(typeof(ApiResponse<Model.Entity.Hospital>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHospitalAsync(HospitalRequest request, [FromRoute] long HospitalId, [FromHeader] long UserId, [FromHeader] long? AuthenticationType, [FromHeader] string? AuthenticationFor, [FromHeader] string? OtpCode)
        {
            try
            {
                var validationResult = await _hospitalValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = ApiResponse<List<string>>.BadRequest("Validation Error", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                    return BadRequest(errors);
                }
                var result = await _hospitalService.UpdateHospitalAsync(request, HospitalId);
                return StatusCode(result.StatusCode, result);
            }
            catch
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
    }
}

