using FluentValidation;
using SIFO.Model.Request;
using SIFO.Model.Response;
using SIFO.Model.Constant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SIFO.APIService.Master.Service.Contracts;

namespace SIFO.APIService.Master.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.ROLE_SUPER_ADMIN)]
    public class MasterController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly IStateService _stateService;
        private readonly ICityService _cityService;
        private readonly IValidator<CountryRequest> _countryValidator;
        private readonly IValidator<StateRequest> _stateValidator;
        private readonly IValidator<CityRequest> _cityValidator;

        public MasterController(ICountryService countryService, IStateService stateService, ICityService cityService, IValidator<CountryRequest> countryValidator,
            IValidator<StateRequest> stateValidator, IValidator<CityRequest> cityValidator)
        {
             _countryService = countryService;
            _stateService = stateService;
            _cityService = cityService;
            _countryValidator = countryValidator;
            _stateValidator = stateValidator;
            _cityValidator = cityValidator;
        }

        [HttpGet]
        [Route("Countries")]
        public async Task<IActionResult> GetAllCountryAsync([FromHeader] int pageNo = 1, [FromHeader] int pageSize = 10, [FromHeader] string filter = "", [FromHeader] string sortColumn = "Id", [FromHeader] string sortDirection = "DESC", [FromHeader] bool isAll = false)
        {
            try
            {
                var result = await _countryService.GetAllCountryAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpGet]
        [Route("Country/{id}")]
        public async Task<IActionResult> GetCountryByIdAsync(long id)
        {
            try
            {
                var result = await _countryService.GetCountryByIdAsync(id);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPost]
        [Route("Countries")]
        public async Task<IActionResult> CreateCountryAsync(CountryRequest request)
        {
            try
            {
                var validationResult = await _countryValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(errors);
                }
                var result = await _countryService.CreateCountryAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPut]
        [Route("Countries")]
        public async Task<IActionResult> UpdateCountryAsync(CountryRequest request)
        {
            try
            {
                var validationResult = await _countryValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(errors);
                }
                var result = await _countryService.UpdateCountryAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpDelete]
        [Route("Country")]
        public async Task<IActionResult> DeleteCountryAsync(long id)
        {
            var result = await _countryService.DeleteCountryAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        [Route("States")]
        public async Task<IActionResult> GetAllStateAsync([FromHeader] int pageNo = 1, [FromHeader] int pageSize = 10, [FromHeader] string filter = "", [FromHeader] string sortColumn = "Id", [FromHeader] string sortDirection = "DESC", [FromHeader] bool isAll = false)
        {
            try
            {
                var result = await _stateService.GetAllStateAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpGet]
        [Route("State/{id}")]
        public async Task<IActionResult> GetStateByIdAsync(long id)
        {
            try
            {
                var result = await _stateService.GetStateByIdAsync(id);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPost]
        [Route("states")]
        public async Task<IActionResult> CreateStateAsync(StateRequest request)
        {
            try
            {
                var validationResult = await _stateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(errors);
                }
                var result = await _stateService.CreateStateAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPut]
        [Route("States")]
        public async Task<IActionResult> UpdateStateAsync(StateRequest request)
        {
            try
            {
                var validationResult = await _stateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(errors);
                }
                var result = await _stateService.UpdateStateAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpDelete]
        [Route("State")]
        public async Task<IActionResult> DeleteStateAsync(long id)
        {
            var result = await _stateService.DeleteStateAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        [Route("Cities")]
        public async Task<IActionResult> GetAllCityAsync([FromHeader] int pageNo = 1, [FromHeader] int pageSize = 10, [FromHeader] string filter = "", [FromHeader] string sortColumn = "Id", [FromHeader] string sortDirection = "DESC", [FromHeader] bool isAll = false)
        {
            try
            {
                var result = await _cityService.GetAllCityAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpGet]
        [Route("City/{id}")]
        public async Task<IActionResult> GetCityByIdAsync(long id)
        {
            try
            {
                var result = await _cityService.GetCityByIdAsync(id);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPost]
        [Route("Cities")]
        public async Task<IActionResult> CreateCityAsync(CityRequest request)
        {
            try
            {
                var validationResult = await _cityValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(errors);
                }
                var result = await _cityService.CreateCityAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPut]
        [Route("Cities")]
        public async Task<IActionResult> UpdateCityAsync(CityRequest request)
        {
            try
            {
                var validationResult = await _cityValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(errors);
                }
                var result = await _cityService.UpdateCityAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpDelete]
        [Route("City")]
        public async Task<IActionResult> DeleteCityAsync(long id)
        {
            var result = await _cityService.DeleteCityAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        [Route("StateByCountry/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStateByCountryIdAsync(long id)
        {
            try
            {
                var result = await _stateService.GetStateByCountryIdAsync(id);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpGet]
        [Route("CityByState/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCityByStateIdAsync(long id)
        {
            try
            {
                var result = await _cityService.GetCityByStateIdAsync(id);
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

