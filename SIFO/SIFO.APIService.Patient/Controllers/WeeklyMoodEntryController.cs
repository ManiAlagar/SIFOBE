using SIFO.Model.Entity;
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
    public class WeeklyMoodEntryController : ControllerBase
    {
        private readonly IWeeklyMoodEntryService _weeklyMoodEntryService;

        public WeeklyMoodEntryController(IWeeklyMoodEntryService weeklyMoodEntryService)
        {
            _weeklyMoodEntryService = weeklyMoodEntryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<WeeklyMoodEntryResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllWeeklyMoodEntryAsync([FromHeader] DateTime? startDate, [FromHeader] DateTime? endDate, [FromHeader] long patientId)
        {
            try
            {
                var result = await _weeklyMoodEntryService.GetAllWeeklyMoodEntryAsync(startDate, endDate, patientId);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpGet]
        [Route("{weeklyMoodEntryId}")]
        [ProducesResponseType(typeof(ApiResponse<WeeklyMoodEntryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetWeeklyMoodEntryByIdAsync([FromRoute] long weeklyMoodEntryId)
        {
            try
            {
                var result = await _weeklyMoodEntryService.GetWeeklyMoodEntryByIdAsync(weeklyMoodEntryId);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<WeeklyMoodEntry>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateWeeklyMoodEntryAsync(WeeklyMoodEntryRequest request)
        {
            try
            {
                var result = await _weeklyMoodEntryService.CreateWeeklyMoodEntryAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPut]
        [Route("{weeklyMoodEntryId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateWeeklyMoodEntryAsync(WeeklyMoodEntryRequest request, [FromRoute] long weeklyMoodEntryId, [FromHeader] long PatientId, [FromHeader] long? AuthenticationType, [FromHeader] string? AuthenticationFor, [FromHeader] string? OtpCode)
        {
            try
            {
                request.Id = weeklyMoodEntryId;
                request.PatientId = PatientId;
                var result = await _weeklyMoodEntryService.UpdateWeeklyMoodEntryAsync(request, weeklyMoodEntryId);
                return StatusCode(result.StatusCode, result);
            }
            catch
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpDelete]
        [Route("{weeklyMoodEntryId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteWeeklyMoodEntryAsync([FromRoute] long weeklyMoodEntryId)
        {
            try
            {
                var result = await _weeklyMoodEntryService.DeleteWeeklyMoodEntryAsync(weeklyMoodEntryId);
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