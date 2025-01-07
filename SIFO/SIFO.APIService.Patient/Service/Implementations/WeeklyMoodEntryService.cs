using AutoMapper;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Constant;
using SIFO.Model.Response;
using SIFO.Common.Contracts;
using SIFO.APIService.Patient.Service.Contracts;
using SIFO.APIService.Patient.Repository.Contracts;

namespace SIFO.APIService.Patient.Service.Implementations
{
    public class WeeklyMoodEntryService : IWeeklyMoodEntryService
    {
        private readonly IWeeklyMoodEntryRepository _weeklyMoodEntryRepository;
        private readonly ICommonService _commonService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public WeeklyMoodEntryService(IWeeklyMoodEntryRepository weeklyMoodEntryRepository, ICommonService commonService, IMapper mapper, IConfiguration configuration)
        {
            _weeklyMoodEntryRepository = weeklyMoodEntryRepository;
            _commonService = commonService;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ApiResponse<Dictionary<string,WeeklyMoodEntryResponse>>> GetAllWeeklyMoodEntryAsync(DateTime? startDate, DateTime? endDate, long patientId)
        {
            Dictionary<string, WeeklyMoodEntryResponse> result = new();
            if (startDate is null && endDate is null)
            {
                var weekResult = await _commonService.GetWeeksBetweenDates(DateTime.UtcNow.AddDays(-21), DateTime.UtcNow);
                foreach (var week in weekResult)
                {
                    var response = await _weeklyMoodEntryRepository.GetAllWeeklyMoodEntryAsync(week.WeekStart.Date, week.WeekEnd.Date, patientId);
                    result[$"{week.WeekStart.Date:dd-MM-yyyy} to {week.WeekEnd.Date:dd-MM-yyyy}"] = response;
                }
            }
            else
            {
                var response = await _weeklyMoodEntryRepository.GetAllWeeklyMoodEntryAsync(startDate, endDate, patientId);
                result[$"{startDate.Value.Date:dd-MM-yyyy} to {endDate.Value.Date:dd-MM-yyyy}"] = response;
            }
            return ApiResponse<Dictionary<string, WeeklyMoodEntryResponse>>.Success("success", result);
        }

        public async Task<ApiResponse<WeeklyMoodEntryResponse>> GetWeeklyMoodEntryByIdAsync(long weeklyMoodEntryId)
        {
            if (weeklyMoodEntryId <= 0)
                return ApiResponse<WeeklyMoodEntryResponse>.BadRequest();

            var response = await _weeklyMoodEntryRepository.GetWeeklyMoodEntryByIdAsync(weeklyMoodEntryId);
            if (response != null)
                return ApiResponse<WeeklyMoodEntryResponse>.Success(Constants.SUCCESS, response);

            return ApiResponse<WeeklyMoodEntryResponse>.NotFound();
        }

        public async Task<ApiResponse<string>> CreateWeeklyMoodEntryAsync(WeeklyMoodEntryRequest request)
        {
            var weelyMoodData = await _weeklyMoodEntryRepository.AlreadyOccurringForTheWeek(request.WeekStartDate.Date, request.WeekEndDate.Date, request.PatientId);
            if (weelyMoodData is not null)
                return ApiResponse<string>.Conflict(Constants.RECORD_EXISTS);

            if (!string.IsNullOrEmpty(request.ImagePath))
            {
                var writtenPath = await _commonService.SaveFileAsync(request.ImagePath, Guid.NewGuid().ToString(), Path.Join(_configuration["FileUploadPath:Path"], $"Users/{request.PatientId}"));
                if (writtenPath == Constants.FILE_NOT_VALID)
                    return ApiResponse<string>.BadRequest(Constants.FILE_NOT_VALID);
                else
                    request.ImagePath = writtenPath;
            }

            var tokenData = await _commonService.GetDataFromToken();
            var mappedResult = _mapper.Map<WeeklyMoodEntry>(request);
            mappedResult.CreatedBy = Convert.ToInt64(tokenData.UserId);
            mappedResult.ImagePath = request.ImagePath;
            mappedResult.CreatedDate = DateTime.UtcNow;

            bool isSuccess = await _weeklyMoodEntryRepository.CreateWeeklyMoodEntryAsync(mappedResult);
            if (isSuccess)
                return ApiResponse<string>.Success(Constants.SUCCESS);

            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<string>> UpdateWeeklyMoodEntryAsync(WeeklyMoodEntryRequest request, long weeklyMoodEntryId)
        {
            var tokenData = await _commonService.GetDataFromToken();
            if (weeklyMoodEntryId <= 0)
                return ApiResponse<string>.BadRequest();

            var response = await _weeklyMoodEntryRepository.GetWeeklyMoodEntryByIdAsync(weeklyMoodEntryId);
            if (response is null)
                return ApiResponse<string>.NotFound(Constants.WEEKLY_MOOD_ENTRY_NOT_FOUND);

            if (!string.IsNullOrEmpty(request.ImagePath))
            {
                var writtenPath = await _commonService.SaveFileAsync(request.ImagePath, Guid.NewGuid().ToString(), Path.Join(_configuration["FileUploadPath:Path"], $"Users/{request.PatientId}"));
                if (writtenPath == Constants.FILE_NOT_VALID)
                    return ApiResponse<string>.BadRequest(Constants.FILE_NOT_VALID);
                else
                    request.ImagePath = writtenPath;
            }

            var mappedResult = _mapper.Map<WeeklyMoodEntry>(request);
            mappedResult.UpdatedDate = DateTime.UtcNow;
            mappedResult.UpdatedBy = Convert.ToInt64(tokenData.UserId);
            mappedResult.ImagePath = request.ImagePath;

            var isSuccess = await _weeklyMoodEntryRepository.UpdateWeeklyMoodEntryAsync(mappedResult, weeklyMoodEntryId);
            if (!isSuccess)
                return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);

            var pathExists = await _commonService.DeleteFileAsync(response.ImagePath);
            if (pathExists != Constants.SUCCESS)
                return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);

            return ApiResponse<string>.Success(Constants.SUCCESS);
        }

        public async Task<ApiResponse<string>> DeleteWeeklyMoodEntryAsync(long weeklyMoodEntryId)
        {
            var response = await _weeklyMoodEntryRepository.DeleteWeeklyMoodEntryAsync(weeklyMoodEntryId);
            if (response == Constants.NOT_FOUND)
                return new ApiResponse<string>(StatusCodes.Status404NotFound, Constants.WEEKLY_MOOD_ENTRY_NOT_FOUND);

            if (response == Constants.DATA_DEPENDENCY_ERROR_MESSAGE)
                return new ApiResponse<string>(StatusCodes.Status400BadRequest, Constants.DATA_DEPENDENCY_ERROR_MESSAGE);

            return ApiResponse<string>.Success(Constants.SUCCESS, response);
        }
    }
}
