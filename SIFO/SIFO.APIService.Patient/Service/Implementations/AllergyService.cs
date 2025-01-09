using AutoMapper;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Constant;
using SIFO.Model.Response;
using SIFO.Common.Contracts;
using SIFO.Utility.Implementations;
using SIFO.APIService.Patient.Service.Contracts;
using SIFO.APIService.Patient.Repository.Contracts;

namespace SIFO.APIService.Patient.Service.Implementations
{
    public class AllergyService : IAllergyService
    {
        private readonly IAllergyRepository _allergyRepository;
        private readonly ICommonService _commonService;
        private readonly IMapper _mapper;

        public AllergyService(IAllergyRepository allergyRepository, ICommonService commonService, IMapper mapper)
        {
            _allergyRepository = allergyRepository;
            _commonService = commonService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<string>> CreateAllergyAsync(AllergyRequest request)
        {
            var isExistsByName = await _allergyRepository.AllergyNameExistsAsync(request.Name, 0);
            if (isExistsByName != 0)
                return ApiResponse<string>.Conflict(Constants.ALLERGY_ALREADY_EXISTS);

            var tokenData = await _commonService.GetDataFromToken();
            var mappedResult = _mapper.Map<Allergy>(request);
            mappedResult.CreatedBy = Convert.ToInt64(tokenData.UserId);
            mappedResult.CreatedDate = DateTime.UtcNow;

            bool isSuccess = await _allergyRepository.CreateAllergyAsync(mappedResult);
            if (isSuccess)
                return ApiResponse<string>.Success(Constants.SUCCESS);

            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<string>> DeleteAllergyAsync(long allergyId)
        {
            if (allergyId <= 0)
                return ApiResponse<string>.BadRequest(Constants.BAD_REQUEST);

            var result = await _allergyRepository.GetAllergyByIdAsync(allergyId);
            if (result is null)
                return ApiResponse<string>.NotFound(Constants.ALLERGY_NOT_FOUND);

            var response = await _allergyRepository.DeleteAllergyAsync(allergyId);
            if (response == Constants.SUCCESS)
                return ApiResponse<string>.Success(Constants.SUCCESS, response);

            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<PagedResponse<AllergyResponse>>> GetAllAllergyAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll)
        {
            var isValid = await HelperService.ValidateGet(pageNo, pageSize, filter, sortColumn, sortDirection);
            if (isValid.Any())
                return ApiResponse<PagedResponse<AllergyResponse>>.BadRequest(isValid[0]);

            var response = await _allergyRepository.GetAllAllergyAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll);
            return ApiResponse<PagedResponse<AllergyResponse>>.Success(Constants.SUCCESS, response);
        }

        public async Task<ApiResponse<AllergyResponse>> GetAllergyByIdAsync(long allergyId)
        {
            if (allergyId <= 0)
                return ApiResponse<AllergyResponse>.BadRequest(Constants.BAD_REQUEST);

            var response = await _allergyRepository.GetAllergyByIdAsync(allergyId);
            if (response != null)
                return ApiResponse<AllergyResponse>.Success(Constants.SUCCESS, response);

            return ApiResponse<AllergyResponse>.NotFound(Constants.NOT_FOUND);
        }

        public async Task<ApiResponse<string>> UpdateAllergyAsync(AllergyRequest request, long allergyId)
        {
            var isExistsByName = await _allergyRepository.AllergyNameExistsAsync(request.Name, allergyId);
            if (isExistsByName != 0)
                return ApiResponse<string>.Conflict(Constants.ALLERGY_ALREADY_EXISTS);

            var tokenData = await _commonService.GetDataFromToken();
            if (allergyId <= 0)
                return ApiResponse<string>.BadRequest();

            var response = await _allergyRepository.GetAllergyByIdAsync(allergyId);
            if (response is null)
                return ApiResponse<string>.NotFound(Constants.ALLERGY_NOT_FOUND);

            var mappedResult = _mapper.Map<Allergy>(request);
            mappedResult.UpdatedDate = DateTime.UtcNow;
            mappedResult.UpdatedBy = Convert.ToInt64(tokenData.UserId);

            var isSuccess = await _allergyRepository.UpdateAllergyAsync(mappedResult, allergyId);
            if (isSuccess)
                return ApiResponse<string>.Success(Constants.SUCCESS);

            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }
    }
}
