using AutoMapper;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;
using SIFO.Model.Constant;
using SIFO.Common.Contracts;
using SIFO.Utility.Implementations;
using SIFO.APIService.Master.Service.Contracts;
using SIFO.APIService.Master.Repository.Contracts;

namespace SIFO.APIService.Master.Service.Implementations
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;
        private readonly ICommonService _commonService;

        public CityService(ICityRepository cityRepository, IMapper mapper,ICommonService commonService)
        {
            _cityRepository = cityRepository;
            _mapper = mapper; 
            _commonService = commonService;
        }

        public async Task<ApiResponse<PagedResponse<CityResponse>>> GetAllCityAsync(int pageNo = 1, int pageSize = 10, string filter = "", string sortColumn = "Id", string sortDirection = "DESC", bool isAll = false)
        {
            var isValid = await HelperService.ValidateGet(pageNo, pageSize, filter, sortColumn, sortDirection);

            if (isValid.Any())
                return ApiResponse<PagedResponse<CityResponse>>.BadRequest(isValid[0]);

            var response = await _cityRepository.GetAllCityAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll);

            return ApiResponse<PagedResponse<CityResponse>>.Success(Constants.SUCCESS, response);
        }

        public async Task<ApiResponse<CityResponse>> GetCityByIdAsync(long id)
        {
            if (id <= 0)
                return ApiResponse<CityResponse>.BadRequest(Constants.BAD_REQUEST);

            var response = await _cityRepository.GetCityByIdAsync(id);

            if (response != null)
                return ApiResponse<CityResponse>.Success(Constants.SUCCESS, response);

            return ApiResponse<CityResponse>.NotFound(Constants.NOT_FOUND);
        }

        public async Task<ApiResponse<List<CityResponse>>> GetCityByStateIdAsync(long stateId)
        {
            if (stateId <= 0)
                return ApiResponse<List<CityResponse>>.BadRequest(Constants.BAD_REQUEST);

            var response = await _cityRepository.GetCityByStateIdAsync(stateId);

            if (response != null)
                return ApiResponse<List<CityResponse>>.Success(Constants.SUCCESS, response);

            return ApiResponse<List<CityResponse>>.NotFound(Constants.NOT_FOUND);
        }

        public async Task<ApiResponse<City>> CreateCityAsync(CityRequest entity)
        {
            bool isNameExists = await _cityRepository.CityExistsByNameAsync(entity.Name);

            if (isNameExists)
                return ApiResponse<City>.Conflict(Constants.CITY_ALREADY_EXISTS);

            var mappedResult = _mapper.Map<City>(entity);
            mappedResult.CountryId = await _commonService.GetCountryIdByCountryCodeAsync(entity.CountryCode);
            var response = await _cityRepository.CreateCityAsync(mappedResult);

            if (response.Id > 0)
                return ApiResponse<City>.Success(Constants.SUCCESS, response);

            return ApiResponse<City>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<City>> UpdateCityAsync(CityRequest entity)
        {
            bool isCityExists = await _cityRepository.CityExistsByIdAsync(entity.Id);
            if (!isCityExists)
                return ApiResponse<City>.NotFound(Constants.CITY_NOT_FOUND);

            var mappedResult = _mapper.Map<City>(entity);
            mappedResult.CountryId = await _commonService.GetCountryIdByCountryCodeAsync(entity.CountryCode);
            var response = await _cityRepository.UpdateCityAsync(mappedResult);

            if (response != null)
                return ApiResponse<City>.Success(Constants.SUCCESS, response);
            return ApiResponse<City>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<string>> DeleteCityAsync(long cityId)
        {
            var response = await _cityRepository.DeleteCityAsync(cityId);
            if (response == Constants.NOT_FOUND)
                return ApiResponse<string>.NotFound(Constants.CITY_NOT_FOUND);
            return ApiResponse<string>.Success(Constants.SUCCESS, response);
        }
    }
}
