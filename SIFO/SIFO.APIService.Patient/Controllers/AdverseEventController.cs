using FluentValidation;
using SIFO.Model.Request;
using SIFO.Model.Response;
using SIFO.Model.Constant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SIFO.APIService.Patient.Service.Contracts;

namespace SIFO.APIService.Patient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{Constants.ROLE_QC_ADMINISTRATOR},{Constants.ROLE_HOSPITAL_PHARMACY_SUPERVISOR},{Constants.ROLE_HOSPITAL_PHARMACY_OPERATOR},{Constants.ROLE_HOSPITAL_REFERENT},{Constants.ROLE_DOCTOR}")]
    public class AdverseEventController : ControllerBase
    {
        private readonly IAdverseEventService _adverseEventService;
        private readonly IValidator<AdverseEventRequest> _adverseEventValidator;

        public AdverseEventController(IAdverseEventService adverseEventService, IValidator<AdverseEventRequest> adverseEventValidator)
        {
            _adverseEventService = adverseEventService;
            _adverseEventValidator = adverseEventValidator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<AdverseEventResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAdverseEventAsync([FromHeader] int pageNo = 1, [FromHeader] int pageSize = 10, [FromHeader] string filter = "", [FromHeader] string sortColumn = "Id", [FromHeader] string sortDirection = "DESC", [FromHeader] bool isAll = false , [FromHeader] long patientId = 0)
        {
            try
            {
                var result = await _adverseEventService.GetAllAdverseEventAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll , patientId);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpGet]
        [Route("{adverseEventId}")]
        [ProducesResponseType(typeof(ApiResponse<AdverseEventResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAdverseEventByIdAsync([FromRoute] long adverseEventId)
        {
            try
            {
                var result = await _adverseEventService.GetAdverseEventByIdAsync(adverseEventId);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAdverseEventAsync(AdverseEventRequest request)
        {
            try
            {
                var validationResult = await _adverseEventValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = ApiResponse<List<string>>.BadRequest("Validation Error", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                    return BadRequest(errors);
                }
                var result = await _adverseEventService.CreateAdverseEventAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPut]
        [Route("{adverseEventId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAdverseEventAsync(AdverseEventRequest request, [FromRoute] long adverseEventId, [FromHeader] long PatientId, [FromHeader] long? AuthenticationType, [FromHeader] string? AuthenticationFor, [FromHeader] string? OtpCode)
        {
            try
            {
                var validationResult = await _adverseEventValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = ApiResponse<List<string>>.BadRequest("Validation Error", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                    return BadRequest(errors);
                }
                request.Id = adverseEventId; 
                request.PatientId = PatientId;
                var result = await _adverseEventService.UpdateAdverseEventAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpDelete]
        [Route("{adverseEventId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAdverseEventAsync([FromRoute] long adverseEventId)
        {
            try
            {
                var result = await _adverseEventService.DeleteAdverseEventAsync(adverseEventId);
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