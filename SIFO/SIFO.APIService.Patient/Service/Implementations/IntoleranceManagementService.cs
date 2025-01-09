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
    public class IntoleranceManagementService : IIntoleranceManagementService
    {
        private readonly IIntoleranceManagementRepository _intoleranceManagementRepository;
        private readonly ICommonService _commonService;
        private readonly IMapper _mapper;

        public IntoleranceManagementService(IIntoleranceManagementRepository intoleranceManagementRepository, ICommonService commonService, IMapper mapper)
        {
            _intoleranceManagementRepository = intoleranceManagementRepository;
            _commonService = commonService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PagedResponse<IntoleranceManagementResponse>>> GetAllIntoleranceManagementAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll)
        {
            var isValid = await HelperService.ValidateGet(pageNo, pageSize, filter, sortColumn, sortDirection);
            if (isValid.Any())
                return ApiResponse<PagedResponse<IntoleranceManagementResponse>>.BadRequest(isValid[0]);

            var response = await _intoleranceManagementRepository.GetAllIntoleranceManagementAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll);
            return ApiResponse<PagedResponse<IntoleranceManagementResponse>>.Success(Constants.SUCCESS, response);
        }

        public async Task<ApiResponse<IntoleranceManagementResponse>> GetIntoleranceManagementByIdAsync(long intoleranceManagementId)
        {
            if (intoleranceManagementId <= 0)
                return ApiResponse<IntoleranceManagementResponse>.BadRequest(Constants.BAD_REQUEST);

            var response = await _intoleranceManagementRepository.GetIntoleranceManagementByIdAsync(intoleranceManagementId);
            if (response != null)
                return ApiResponse<IntoleranceManagementResponse>.Success(Constants.SUCCESS, response);

            return ApiResponse<IntoleranceManagementResponse>.NotFound(Constants.NOT_FOUND);
        }

        public async Task<ApiResponse<string>> DeleteIntoleranceManagementAsync(long intoleranceManagementId)
        {
            if (intoleranceManagementId <= 0)
                return ApiResponse<string>.BadRequest(Constants.BAD_REQUEST);

            var response = await _intoleranceManagementRepository.DeleteIntoleranceManagementAsync(intoleranceManagementId);
            if (response == Constants.NOT_FOUND)
                return ApiResponse<string>.NotFound(Constants.INTOLERANCE_MANAGEMENT_NOT_FOUND);

            return ApiResponse<string>.Success(Constants.SUCCESS, response);
        }

        public async Task<ApiResponse<string>> CreateIntoleranceManagementAsync(IntoleranceManagementRequest request)
        {
            var isExistsByName = await _intoleranceManagementRepository.IntoleranceManagementNameExistsAsync(request.Name, 0);
            if (isExistsByName != 0)
                return ApiResponse<string>.Conflict(Constants.INTOLERANCE_MANAGEMENT_ALREADY_EXISTS);

            var tokenData = await _commonService.GetDataFromToken();
            var mappedResult = _mapper.Map<IntoleranceManagement>(request);
            mappedResult.CreatedBy = Convert.ToInt64(tokenData.UserId);
            mappedResult.CreatedDate = DateTime.UtcNow;

            bool isSuccess = await _intoleranceManagementRepository.CreateIntoleranceManagementAsync(mappedResult);
            if (isSuccess)
                return ApiResponse<string>.Success(Constants.SUCCESS);

            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<string>> UpdateIntoleranceManagementAsync(IntoleranceManagementRequest request, long intoleranceManagementId)
        {
            var isExistsByName = await _intoleranceManagementRepository.IntoleranceManagementNameExistsAsync(request.Name, intoleranceManagementId);
            if (isExistsByName != 0)
                return ApiResponse<string>.Conflict(Constants.INTOLERANCE_MANAGEMENT_ALREADY_EXISTS);

            var tokenData = await _commonService.GetDataFromToken();
            if (intoleranceManagementId <= 0)
                return ApiResponse<string>.BadRequest(Constants.BAD_REQUEST);

            var response = await _intoleranceManagementRepository.GetIntoleranceManagementByIdAsync(intoleranceManagementId);
            if (response is null)
                return ApiResponse<string>.NotFound(Constants.INTOLERANCE_MANAGEMENT_NOT_FOUND);

            var mappedResult = _mapper.Map<IntoleranceManagement>(request);
            mappedResult.UpdatedDate = DateTime.UtcNow;
            mappedResult.UpdatedBy =Convert.ToInt64(tokenData.UserId);

            var isSuccess = await _intoleranceManagementRepository.UpdateIntoleranceManagementAsync(mappedResult, intoleranceManagementId);
            if (isSuccess)
                return ApiResponse<string>.Success(Constants.SUCCESS);

            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }
    }
}
