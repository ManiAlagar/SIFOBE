using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIFO.APIService.Hospital.Service.Contracts;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;
using SIFO.Model.Validator;

namespace SIFO.APIService.Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [ProducesResponseType(typeof(ApiResponse<Hospitals>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateHospitalasync(HospitalRequest request)
        {
            try
            {
                var validationResult = await _hospitalValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
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

        [HttpPut]
        [Route("{HospitalId}")]
        public async Task<IActionResult> UpdateHospitalAsync(HospitalRequest request, [FromRoute] long HospitalId)
        {
            try
            {
                var validationResult = await _hospitalValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
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

