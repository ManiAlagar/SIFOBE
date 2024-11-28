using AutoMapper;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;
using SIFO.Model.Constant;
using SIFO.Utility.Implementations;
using SIFO.APIService.Master.Service.Contracts;
using SIFO.APIService.Master.Repository.Contracts;

namespace SIFO.APIService.Master.Service.Implementations
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;

        public CityService(ICityRepository cityRepository, IMapper mapper)
        {
            _cityRepository = cityRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PagedResponse<CityResponse>>> GetAllCityAsync(int pageNo = 1, int pageSize = 10, string filter = "", string sortColumn = "Id", string sortDirection = "DESC", bool isAll = false)
        {
            var isValid = await HelperService.ValidateGet(pageNo, pageSize, filter, sortColumn, sortDirection);

            if (isValid.Any())
                return ApiResponse<PagedResponse<CityResponse>>.BadRequest();

            var response = await _cityRepository.GetAllCityAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll);

            return ApiResponse<PagedResponse<CityResponse>>.Success(Constants.SUCCESS, response);
        }

        public async Task<ApiResponse<CityResponse>> GetCityByIdAsync(long id)
        {
            if (id <= 0)
                return ApiResponse<CityResponse>.BadRequest();

            var response = await _cityRepository.GetCityByIdAsync(id);

            if (response != null)
                return ApiResponse<CityResponse>.Success(Constants.SUCCESS, response);

            return ApiResponse<CityResponse>.NotFound();
        }

        public async Task<ApiResponse<List<CityResponse>>> GetCityByStateIdAsync(long stateId)
        {
            if (stateId <= 0)
                return ApiResponse<List<CityResponse>>.BadRequest();

            var response = await _cityRepository.GetCityByStateIdAsync(stateId);

            if (response != null)
                return ApiResponse<List<CityResponse>>.Success(Constants.SUCCESS, response);

            return ApiResponse<List<CityResponse>>.NotFound();
        }

        public async Task<ApiResponse<City>> CreateCityAsync(CityRequest entity)
        {
            bool isNameExists = await _cityRepository.CityExistsByNameAsync(entity.Name, entity.Id);

            if (isNameExists)
                return new ApiResponse<City>(StatusCodes.Status409Conflict, Constants.CITY_ALREADY_EXISTS);

            var mappedResult = _mapper.Map<City>(entity);

            var response = await _cityRepository.CreateCityAsync(mappedResult);

            if (response.Id > 0)
                return ApiResponse<City>.Success(Constants.SUCCESS, response);

            return new ApiResponse<City>(StatusCodes.Status500InternalServerError);
        }

        public async Task<ApiResponse<City>> UpdateCityAsync(CityRequest entity)
        {
            bool isCityExists = await _cityRepository.CityExistsByIdAsync(entity.Id);

            if (!isCityExists)
                return new ApiResponse<City>(StatusCodes.Status404NotFound, Constants.CITY_NOT_FOUND);

            var mappedResult = _mapper.Map<City>(entity);

            var response = await _cityRepository.UpdateCityAsync(mappedResult);

            if (response != null)
            {
                return ApiResponse<City>.Success(Constants.SUCCESS, response);
            }
            return new ApiResponse<City>(StatusCodes.Status500InternalServerError);
        }

        public async Task<ApiResponse<string>> DeleteCityAsync(long cityId)
        {
            var response = await _cityRepository.DeleteCityAsync(cityId);
            return ApiResponse<string>.Success(Constants.SUCCESS, response);
        }
    }
}
