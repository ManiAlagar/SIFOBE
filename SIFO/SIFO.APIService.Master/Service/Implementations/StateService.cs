using AutoMapper;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;
using SIFO.Model.Constant;
using SIFO.Utility.Implementations;
using SIFO.APIService.Master.Service.Contracts;
using SIFO.APIService.Master.Repository.Contracts;
using SIFO.Common.Contracts;

namespace SIFO.APIService.Master.Service.Implementations
{
    public class StateService : IStateService
    {

        private readonly IStateRepository _stateRepository;
        private readonly IMapper _mapper;
        private readonly ICommonService _commonService;

        public StateService(IStateRepository stateRepository, IMapper mapper,ICommonService commonService)
        {
            _stateRepository = stateRepository;
            _mapper = mapper; 
            _commonService = commonService;
        }

        public async Task<ApiResponse<PagedResponse<StateResponse>>> GetAllStateAsync(int pageNo = 1, int pageSize = 10, string filter = "", string sortColumn = "Id", string sortDirection = "DESC", bool isAll = false)
        {
            var isValid = await HelperService.ValidateGet(pageNo, pageSize, filter, sortColumn, sortDirection);

            if (isValid.Any())
                return ApiResponse<PagedResponse<StateResponse>>.BadRequest(isValid[0]);

            var response = await _stateRepository.GetAllStateAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll);

            return ApiResponse<PagedResponse<StateResponse>>.Success(Constants.SUCCESS, response);
        }

        public async Task<ApiResponse<StateResponse>> GetStateByIdAsync(long id)
        {
            if (id <= 0)
                return ApiResponse<StateResponse>.BadRequest();

            var response = await _stateRepository.GetStateByIdAsync(id);

            if (response != null)
                return ApiResponse<StateResponse>.Success(Constants.SUCCESS, response);

            return ApiResponse<StateResponse>.NotFound();
        }

        public async Task<ApiResponse<State>> CreateStateAsync(StateRequest entity)
        {
            bool isNameExists = await _stateRepository.StateExistsByNameAsync(entity.Name);

            if (isNameExists)
                return new ApiResponse<State>(StatusCodes.Status409Conflict, Constants.STATE_ALREADY_EXISTS);

            entity.Iso2 = entity.Name.Substring(0,2);

            var mappedResult = _mapper.Map<State>(entity);
            mappedResult.CountryId = await _commonService.GetCountryIdByCountryCodeAsync(entity.CountryCode);
            var response = await _stateRepository.CreateStateAsync(mappedResult);

            if (response.Id > 0)
                return ApiResponse<State>.Success(Constants.SUCCESS, response);

            return new ApiResponse<State>(StatusCodes.Status500InternalServerError);
        }

        public async Task<ApiResponse<State>> UpdateStateAsync(StateRequest entity)
        {
            bool isStateExists = await _stateRepository.StateExistsByIdAsync(entity.Id);

            if (!isStateExists)
                return new ApiResponse<State>(StatusCodes.Status404NotFound,Constants.STATE_NOT_FOUND);

            var mappedResult = _mapper.Map<State>(entity);
            mappedResult.CountryId = await _commonService.GetCountryIdByCountryCodeAsync(entity.CountryCode);
            var response = await _stateRepository.UpdateStateAsync(mappedResult);

            if (response != null)
            {
                return ApiResponse<State>.Success(Constants.SUCCESS, response);
            }
            return new ApiResponse<State>(StatusCodes.Status500InternalServerError);
        }

        public async Task<ApiResponse<string>> DeleteStateAsync(long id)
        {
            var response = await _stateRepository.DeleteStateAsync(id);
            if (response == Constants.NOT_FOUND)
                return new ApiResponse<string>(StatusCodes.Status404NotFound, Constants.STATE_NOT_FOUND);
            return ApiResponse<string>.Success(Constants.SUCCESS, response);
        }

        public async Task<ApiResponse<List<StateResponse>>> GetStateByCountryIdAsync(string countryCode)
        {
            if (string.IsNullOrEmpty(countryCode))
                return ApiResponse<List<StateResponse>>.BadRequest();

            var response = await _stateRepository.GetStateByCountryIdAsync(countryCode);

            if (response != null)
                return ApiResponse<List<StateResponse>>.Success(Constants.SUCCESS, response);

            return ApiResponse<List<StateResponse>>.NotFound();
        }
    }
}
