using FluentValidation;
using SIFO.Model.Request;
using SIFO.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SIFO.APIService.Hospital.Service.Contracts;
using SIFO.Model.Constant;

namespace SIFO.APIService.Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacyController : ControllerBase
    {
        private readonly IPharmacyService _pharmacyService;
        private readonly IValidator<PharmacyRequest> _pharmacyValidator;

        public PharmacyController(IPharmacyService pharmacyService, IValidator<PharmacyRequest> pharmacyValidator)
        {
            _pharmacyService = pharmacyService;
            _pharmacyValidator = pharmacyValidator;
        }

        [HttpGet]
        [Route("{pharmacyId}")]
        [Authorize(Roles =$"{Constants.ROLE_QC_ADMINISTRATOR}, {Constants.ROLE_HOSPITAL_PHARMACY_OPERATOR}, {Constants.ROLE_HOSPITAL_PHARMACY_SUPERVISOR}, {Constants.ROLE_RETAIL_PHARMACY_OPERATOR},{Constants.ROLE_RETAIL_PHARMACY_SUPERVISOR}")]
        [ProducesResponseType(typeof(ApiResponse<PharmacyDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPharmacyByIdAsync([FromRoute] long pharmacyId)
        {
            try
            {
                var result = await _pharmacyService.GetPharmacyByIdAsync(pharmacyId);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPost]
        [Authorize(Roles = Constants.ROLE_QC_ADMINISTRATOR)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePharmacyAsync(PharmacyRequest request)
        {
            try
            {
                var validationResult = await _pharmacyValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = ApiResponse<List<string>>.BadRequest("Validation Error", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                    return BadRequest(errors);
                }
                var result = await _pharmacyService.CreatePharmacyAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPut]
        [Route("{pharmacyId}")]
        [Authorize(Roles = $"{Constants.ROLE_QC_ADMINISTRATOR}, {Constants.ROLE_HOSPITAL_PHARMACY_OPERATOR}, {Constants.ROLE_HOSPITAL_PHARMACY_SUPERVISOR}, {Constants.ROLE_RETAIL_PHARMACY_OPERATOR},{Constants.ROLE_RETAIL_PHARMACY_SUPERVISOR}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePharmacyAsync(PharmacyRequest request, [FromRoute] long pharmacyId, [FromHeader] long UserId, [FromHeader] long? AuthenticationType, [FromHeader] string? AuthenticationFor, [FromHeader] string? OtpCode)
        {
            try
            {
                var validationResult = await _pharmacyValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = ApiResponse<List<string>>.BadRequest("Validation Error", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                    return BadRequest(errors);
                }
                var result = await _pharmacyService.UpdatePharmacyAsync(request, pharmacyId);
                return StatusCode(result.StatusCode, result);
            }
            catch
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpDelete]
        [Route("{pharmacyId}")]
        [Authorize(Roles = Constants.ROLE_QC_ADMINISTRATOR)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePharmacyAsync([FromRoute] long pharmacyId)
        {
            try
            {
                var result = await _pharmacyService.DeletePharmacyAsync(pharmacyId);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpGet]
        [Route("/api/pharmacies")]
        [Authorize(Roles = $"{Constants.ROLE_QC_ADMINISTRATOR}, {Constants.ROLE_HOSPITAL_PHARMACY_OPERATOR}, {Constants.ROLE_HOSPITAL_PHARMACY_SUPERVISOR}, {Constants.ROLE_RETAIL_PHARMACY_OPERATOR},{Constants.ROLE_RETAIL_PHARMACY_SUPERVISOR}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<PharmaciesResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPharmacyAsync([FromHeader] int pageNo = 1, [FromHeader] int pageSize = 10, [FromHeader] string filter = "", [FromHeader] string sortColumn = "Id", [FromHeader] string sortDirection = "DESC", [FromHeader] bool isAll = false, [FromHeader] string? pharmacyType = null, [FromHeader] bool isCurrentUser = true)
        {
            try
            {
                var result = await _pharmacyService.GetPharmacyAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll, pharmacyType, isCurrentUser);
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
