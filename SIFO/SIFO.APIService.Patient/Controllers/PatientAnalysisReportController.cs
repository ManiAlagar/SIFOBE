using FluentValidation;
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
    public class PatientAnalysisReportController : ControllerBase
    {
        private readonly IPatientAnalysisReportService _patientAnalysisReportService;
        private readonly IValidator<PatientAnalysisReportRequest> _patientAnalysisReportValidator;

        public PatientAnalysisReportController(IPatientAnalysisReportService patientAnalysisReportService, IValidator<PatientAnalysisReportRequest> patientAnalysisReportValidator)
        {
            _patientAnalysisReportService = patientAnalysisReportService;
            _patientAnalysisReportValidator = patientAnalysisReportValidator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<PatientAnalysisReportResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllPatientAnalysisReportAsync([FromHeader] int pageNo = 1, [FromHeader] int pageSize = 10, [FromHeader] string filter = "", [FromHeader] string sortColumn = "Id", [FromHeader] string sortDirection = "DESC", [FromHeader] bool isAll = false, [FromHeader] long patientId = 0)
        {
            try
            {
                var result = await _patientAnalysisReportService.GetAllPatientAnalysisReportAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll, patientId);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
        
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PatientAnalysisReport>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePatientAnalysisReportAsync(PatientAnalysisReportRequest request)
        {
            try
            {
                var validationResult = await _patientAnalysisReportValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = ApiResponse<List<string>>.BadRequest("Validation Error", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                    return BadRequest(errors);
                }
                var result = await _patientAnalysisReportService.CreatePatientAnalysisReportAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpDelete]
        [Route("{patientAnalysisReportId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePatientAnalysisReportAsync([FromRoute] long patientAnalysisReportId)
        {
            try
            {
                var result = await _patientAnalysisReportService.DeletePatientAnalysisReportAsync(patientAnalysisReportId);
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
