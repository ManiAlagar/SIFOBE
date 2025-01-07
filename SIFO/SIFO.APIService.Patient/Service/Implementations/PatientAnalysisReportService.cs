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
    public class PatientAnalysisReportService : IPatientAnalysisReportService
    {
        private readonly IPatientAnalysisReportRepository _patientAnalysisReportRepository;
        private readonly ICommonService _commonService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PatientAnalysisReportService(IPatientAnalysisReportRepository patientAnalysisReportRepository, ICommonService commonService, IMapper mapper, IConfiguration configuration)
        {
            _patientAnalysisReportRepository = patientAnalysisReportRepository;
            _commonService = commonService;
            _mapper = mapper;
            _configuration = configuration;

        }
        public async Task<ApiResponse<PagedResponse<PatientAnalysisReportResponse>>> GetAllPatientAnalysisReportAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, long patientId)
        {
            var isValid = await HelperService.ValidateGet(pageNo, pageSize, filter, sortColumn, sortDirection);
            if (isValid.Any())
                return ApiResponse<PagedResponse<PatientAnalysisReportResponse>>.BadRequest(isValid[0]);

            var response = await _patientAnalysisReportRepository.GetAllPatientAnalysisReportAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll, patientId);
            return ApiResponse<PagedResponse<PatientAnalysisReportResponse>>.Success(Constants.SUCCESS, response);
        }

        public async Task<ApiResponse<string>> CreatePatientAnalysisReportAsync(PatientAnalysisReportRequest request)
        {
            var tokenData = await _commonService.GetDataFromToken();

            if (!string.IsNullOrEmpty(request.File))
            {
                var writtenPath = await _commonService.SaveFileAsync(request.File, Guid.NewGuid().ToString(), Path.Join(_configuration["FileUploadPath:Path"], $"Patient/{request.PatientId}"));
                if (writtenPath is null)
                    return ApiResponse<string>.BadRequest("file is invalid");
                else
                    request.File = writtenPath;
            }
            var mappedResult = _mapper.Map<PatientAnalysisReport>(request);
            mappedResult.CreatedBy = Convert.ToInt64(tokenData.UserId);
            mappedResult.FilePath = request.File;
            mappedResult.CreatedDate = DateTime.UtcNow;

            bool isSuccess = await _patientAnalysisReportRepository.CreatePatientAnalysisReportAsync(mappedResult);
            if (isSuccess)
                return ApiResponse<string>.Success(Constants.SUCCESS);

            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<string>> DeletePatientAnalysisReportAsync(long patientAnalysisReportId)
        {
            var patientAnalticsData = await _patientAnalysisReportRepository.GetPatientAnalsisReportByIdAsync(patientAnalysisReportId);  
            if (patientAnalticsData is null)
                return new ApiResponse<string>(StatusCodes.Status404NotFound, Constants.PATIENT_ANALYSIS_REPORT_NOT_FOUND);

            var response = await _patientAnalysisReportRepository.DeletePatientAnalysisReportAsync(patientAnalysisReportId);
            if (response == Constants.DATA_DEPENDENCY_ERROR_MESSAGE)
                return new ApiResponse<string>(StatusCodes.Status400BadRequest, Constants.DATA_DEPENDENCY_ERROR_MESSAGE);
            response = await _commonService.DeleteFileAsync(patientAnalticsData.FilePath);
            if(response == Constants.SUCCESS)
                return ApiResponse<string>.Success(Constants.SUCCESS, response);
            return ApiResponse<string>.InternalServerError();
        }
    }
}
