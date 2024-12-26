﻿using AutoMapper;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Constant;
using SIFO.Model.Response;
using SIFO.Utility.Implementations;
using SIFO.APIService.Master.Service.Contracts;
using SIFO.APIService.Master.Repository.Contracts;

namespace SIFO.APIService.Master.Service.Implementations
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public CountryService(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PagedResponse<CountryResponse>>> GetAllCountryAsync(int pageNo = 1, int pageSize = 10, string filter = "", string sortColumn = "Id", string sortDirection = "DESC", bool isAll = false)
        {
            var isValid = await HelperService.ValidateGet(pageNo, pageSize, filter, sortColumn, sortDirection);

            if (isValid.Any())
                return ApiResponse<PagedResponse<CountryResponse>>.BadRequest(isValid[0]);

            var response = await _countryRepository.GetAllCountryAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll);

            return ApiResponse<PagedResponse<CountryResponse>>.Success(Constants.SUCCESS, response);
        }

        public async Task<ApiResponse<CountryResponse>> GetCountryByIdAsync(string countryCode)
        {
            if (string.IsNullOrEmpty(countryCode))
                return ApiResponse<CountryResponse>.BadRequest();

            var response = await _countryRepository.GetCountryByIdAsync(countryCode);

            if (response != null)
                return ApiResponse<CountryResponse>.Success(Constants.SUCCESS, response);

            return ApiResponse<CountryResponse>.NotFound();
        }

        public async Task<ApiResponse<Country>> CreateCountryAsync(CountryRequest entity)
        {
            bool isNameExists = await _countryRepository.CountryExistsByNameAsync(entity.Name);

            if (isNameExists)
                return new ApiResponse<Country>(StatusCodes.Status409Conflict, Constants.COUNTRY_ALREADY_EXISTS);

            //entity.Iso2 = entity.Name.Substring(0, 2);
            entity.Iso3 = entity.Name.Substring(0,3);

            var mappedResult = _mapper.Map<Country>(entity);

            var response = await _countryRepository.CreateCountryAsync(mappedResult);

            if (response.Id > 0)
                return ApiResponse <Country>.Success(Constants.SUCCESS, response);

            return new ApiResponse<Country>(StatusCodes.Status500InternalServerError);
        }

        public async Task<ApiResponse<Country>> UpdateCountryAsync(CountryRequest entity)
        {
            bool isCountryExists = await _countryRepository.CountryExistsByIdAsync(entity.Iso2);

            if (!isCountryExists)
                return new ApiResponse<Country>(StatusCodes.Status404NotFound, Constants.COUNTRY_NOT_FOUND);

            var mappedResult = _mapper.Map<Country>(entity);

            var response = await _countryRepository.UpdateCountryAsync(mappedResult);

            if (response != null)   
                return ApiResponse<Country>.Success(Constants.SUCCESS, response);

            return new ApiResponse<Country>(StatusCodes.Status500InternalServerError);
        }

        public async Task<ApiResponse<string>> DeleteCountryAsync(string countryCode)
        {
            var response = await _countryRepository.DeleteCountryAsync(countryCode);
            if (response == Constants.NOT_FOUND)
                return new ApiResponse<string>(StatusCodes.Status404NotFound, Constants.COUNTRY_NOT_FOUND);
            if (response == Constants.SUCCESS)
                return ApiResponse<string>.Success(Constants.SUCCESS, response); 
            return ApiResponse<string>.BadRequest(response);
        }
    }
}