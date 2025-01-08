using FluentValidation;
using SIFO.Model.Constant;
using SIFO.Model.Request;
using SIFO.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SIFO.APIService.Patient.Service.Contracts;

namespace SIFO.APIService.Patient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IValidator<PatientRequest> _patientValidator;

        public PatientController(IPatientService patientService, IValidator<PatientRequest> patientValidator)
        {
            _patientService = patientService;
            _patientValidator = patientValidator;
        }

        [HttpGet]
        [Authorize(Roles=$"{Constants.ROLE_QC_ADMINISTRATOR},{Constants.ROLE_QC_OPERATOR},{Constants.ROLE_DOCTOR},{Constants.ROLE_HOSPITAL_PHARMACY_SUPERVISOR},{Constants.ROLE_RETAIL_PHARMACY_OPERATOR},{Constants.ROLE_RETAIL_PHARMACY_SUPERVISOR},{Constants.ROLE_HOSPITAL_PHARMACY_OPERATOR}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<PatientResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPatientAsync([FromHeader] int pageNo = 1, [FromHeader] int pageSize = 10, [FromHeader] string filter = "", [FromHeader] string sortColumn = "Id", [FromHeader] string sortDirection = "DESC", [FromHeader] bool isAll = false)
        {
            try
            {
                var result = await _patientService.GetPatientAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError();
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<PatientResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPatientByIdAsync([FromRoute] long id)
        {
            try
            {
                var result = await _patientService.GetPatientByIdAsync(id);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError();
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePatientAsync(PatientRequest request)
        {
            try
            {
                var validationResult = await _patientValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = ApiResponse<List<string>>.BadRequest("Validation Error", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                    return BadRequest(errors);
                }
                var result = await _patientService.CreatePatientAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError();
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePatientAsync([FromRoute] long id, [FromBody] PatientRequest request)
        {
            try
            { 
                request.Id = id;
                var result = await _patientService.UpdatePatientAsync(request);
                return StatusCode(result.StatusCode, result); 
            }
            catch (Exception)
            {
                var result = ApiResponse<string>.InternalServerError();
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePatientAsync([FromRoute] long id)
        {
            try
            {
                var result = await _patientService.DeletePatientAsync(id);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPost]
        [Route("Register")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterPatientAsync(RegisterPatientRequest request)
        {
            try
            {
                var result = await _patientService.RegisterPatient(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPost]
        [Route("Verify")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VerifyPatientAsync(VerifyPatientRequest request)
        {
            try
            {
                var result = await _patientService.VerifyPatientAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPost]
        [Route("createPassword")]
        [Authorize(Roles = $"{Constants.ROLE_PATIENT}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePasswordAsync(CreatePasswordRequest request)
        {
            try
            {
                var result = await _patientService.CreatePasswordAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
        [HttpPut]
        [Route("changePassword")]
        [Authorize(Roles = $"{Constants.ROLE_PATIENT}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequest request)
        {
            try
            {
                var result = await _patientService.ChangePasswordAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPost]
        [Route("SendOtp")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendOtpAsync(PatientOtpRequest request)
        {
            try
            {
                var result = await _patientService.SendOtpAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
    }
}
