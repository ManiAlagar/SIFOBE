using AutoMapper;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;
using SIFO.Model.Constant;
using SIFO.Common.Contracts;
using SIFO.Utility.Implementations;
using SIFO.APIService.Patient.Service.Contracts;
using SIFO.APIService.Patient.Repository.Contracts;

namespace SIFO.APIService.Patient.Service.Implementations
{
    public class AdverseEventService : IAdverseEventService
    {
        private readonly IAdverEventRepository _adverseEventRepository;
        private readonly ICommonService _commonService;
        private readonly IMapper _mapper;

        public AdverseEventService(IAdverEventRepository adverseEventRepository, ICommonService commonService, IMapper mapper)
        {
            _adverseEventRepository = adverseEventRepository;
            _commonService = commonService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PagedResponse<AdverseEventResponse>>> GetAllAdverseEventAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, long patientId)
        {
            var isValid = await HelperService.ValidateGet(pageNo, pageSize, filter, sortColumn, sortDirection);
            if (isValid.Any())
                return ApiResponse<PagedResponse<AdverseEventResponse>>.BadRequest(isValid[0]);

            var response = await _adverseEventRepository.GetAllAdverseEventAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll, patientId);
            return ApiResponse<PagedResponse<AdverseEventResponse>>.Success(Constants.SUCCESS, response);
        }

        public async Task<ApiResponse<AdverseEventResponse>> GetAdverseEventByIdAsync(long adverseEventId)
        {
            if (adverseEventId <= 0)
                return ApiResponse<AdverseEventResponse>.BadRequest();

            var response = await _adverseEventRepository.GetAdverseEventByIdAsync(adverseEventId);
            if (response != null)
                return ApiResponse<AdverseEventResponse>.Success(Constants.SUCCESS, response);

            return ApiResponse<AdverseEventResponse>.NotFound();
        }

        public async Task<ApiResponse<string>> CreateAdverseEventAsync(AdverseEventRequest request)
        {
            var adverseResponse = await _adverseEventRepository.AdverseEventNameExistsAsync(request.Name,request.PatientId, 0); 
            if (adverseResponse is not null)
                return ApiResponse<string>.Conflict();

            if (request.Intensity is not null)
            {
                if (!Enum.IsDefined(typeof(Constants.Intensity), request.Intensity.ToLower()))
                    return ApiResponse<string>.BadRequest();
            }

            var tokenData = await _commonService.GetDataFromToken();
            var mappedResult = _mapper.Map<AdverseEvent>(request);
            mappedResult.CreatedBy = Convert.ToInt64(tokenData.UserId);
            mappedResult.CreatedDate = DateTime.UtcNow;

            string response = await _adverseEventRepository.CreateAdverseEventAsync(mappedResult);
            if (response == Constants.SUCCESS)
                return ApiResponse<string>.Success(Constants.SUCCESS);

            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<string>> UpdateAdverseEventAsync(AdverseEventRequest request)
        {
            var isAdverseDataExists = await _adverseEventRepository.GetAdverseEventByIdAsync(request.Id); 
            if(isAdverseDataExists is null)
                return ApiResponse<string>.NotFound();
            
            var adverseResponse = await _adverseEventRepository.AdverseEventNameExistsAsync(request.Name,request.PatientId, request.Id); 
            if (adverseResponse is not null)
                return ApiResponse<string>.Conflict();

            if (request.Intensity is not null)
            {
                if (!Enum.IsDefined(typeof(Constants.Intensity), request.Intensity.ToLower()))
                    return ApiResponse<string>.BadRequest();
            }

            var tokenData = await _commonService.GetDataFromToken();
            var mappedResult = _mapper.Map<AdverseEvent>(request);
            mappedResult.UpdatedDate = DateTime.UtcNow;
            mappedResult.UpdatedBy = Convert.ToInt64(tokenData.UserId);

            string response = await _adverseEventRepository.UpdateAdverseEventAsync(mappedResult);
            if (response == Constants.SUCCESS)
                return ApiResponse<string>.Success(Constants.SUCCESS);

            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<string>> DeleteAdverseEventAsync(long adverseEventId)
        {
            var adverseResponse = await _adverseEventRepository.GetAdverseEventByIdAsync(adverseEventId);
            if (adverseResponse is null)
                return ApiResponse<string>.NotFound();

            string response = await _adverseEventRepository.DeleteAdverseEventAsync(adverseEventId);
            if (response == Constants.SUCCESS)
                return ApiResponse<string>.Success(Constants.SUCCESS);
            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }
    }
}