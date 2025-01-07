using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIFO.APIService.Hospital.Service.Contracts;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrugController : ControllerBase
    {
        private readonly IValidator<DrugRequest> _validator;
        private readonly IDrugService _drugService;
        public DrugController(IValidator<DrugRequest> validator, IDrugService drugService)
        {
            _validator = validator;
            _drugService = drugService;
        }
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(ApiResponse<Model.Entity.Drugs>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateDrugAsync(DrugRequest drugRequest)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(drugRequest);
                if (!validationResult.IsValid)
                {
                    var errors = ApiResponse<List<string>>.BadRequest("Validation Error", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                    return BadRequest(errors);
                }
                var response = await _drugService.CreateDrugAsync(drugRequest);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Model.Entity.Drugs>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateDrugAsync(DrugRequest drugRequest, [FromRoute] long id)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(drugRequest);
                if (!validationResult.IsValid)
                {
                    var errors = ApiResponse<List<string>>.BadRequest("Validation Error", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                    return BadRequest(errors);
                }
                var response = await _drugService.UpdateDrugAsync(drugRequest, id);
                return StatusCode(response.StatusCode, response);
            }
            catch
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
        [HttpGet]
        [Route("{Id}")]
        [ProducesResponseType(typeof(ApiResponse<DrugResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDrugByIdWithRegionsAsync(long Id)
        {
            try
            {
                var result = await _drugService.GetDrugByIdWithRegionsAsync(Id);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
        //[HttpGet]
        //[Route("")]
        //[ProducesResponseType(typeof(ApiResponse<DrugResponse>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> GetAllDrugs([FromHeader] int pageNo = 1, [FromHeader] int pageSize = 10, [FromHeader] string filter = "", [FromHeader] string sortColumn = "Id", [FromHeader] string sortDirection = "DESC", [FromHeader] bool isAll = false)
        //{
        //    try
        //    {
        //        var result = await _drugService.GetAllDrugs(pageNo, pageSize, filter, sortColumn, sortDirection, isAll);
        //        return StatusCode(result.StatusCode, result);
        //    }
        //    catch (Exception ex)
        //    {
        //        var result = ApiResponse<string>.InternalServerError;
        //        return StatusCode(StatusCodes.Status500InternalServerError, result);
        //    }
        //}
        [HttpDelete]
        [Route("{Id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHospitalAsync([FromRoute] long Id)
        {
            try
            {
                var result = await _drugService.DeleteHospitalAsync(Id);
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
