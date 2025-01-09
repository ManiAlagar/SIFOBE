using AutoMapper;
using System.Text;
using Newtonsoft.Json;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Constant;
using SIFO.Model.Response;
using SIFO.Common.Contracts;
using System.Net.Http.Headers;
using SIFO.Utility.Implementations;
using SIFO.APIService.Patient.Service.Contracts;
using SIFO.APIService.Patient.Repository.Contracts;

namespace SIFO.APIService.Patient.Service.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ICommonService _commonService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientService(IPatientRepository PatientRepository, ICommonService commonService, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _patientRepository = PatientRepository;
            _commonService = commonService;
            _mapper = mapper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<PagedResponse<PatientResponse>>> GetPatientAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll)
        {
            var tokenData = await _commonService.GetDataFromToken();

            var isValid = await HelperService.ValidateGet(pageNo, pageSize, filter, sortColumn, sortDirection);
            if (isValid.Any())
                return ApiResponse<PagedResponse<PatientResponse>>.BadRequest(isValid[0]);

            var response = await _patientRepository.GetPatientAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll, tokenData.Role);
            return ApiResponse<PagedResponse<PatientResponse>>.Success(Constants.SUCCESS, response);
        }

        public async Task<ApiResponse<PatientResponse>> GetPatientByIdAsync(long patientId)
        {
            if (patientId <= 0)
                return ApiResponse<PatientResponse>.BadRequest(Constants.BAD_REQUEST);

            var response = await _patientRepository.GetPatientByIdAsync(patientId);

            if (response != null)
                return ApiResponse<PatientResponse>.Success(Constants.SUCCESS, response);

            return ApiResponse<PatientResponse>.NotFound(Constants.NOT_FOUND);
        }

        public async Task<ApiResponse<string>> CreatePatientAsync(PatientRequest request)
        {
            var tokenData = await _commonService.GetDataFromToken();
            var isPatientExists = await _patientRepository.PhoneNumberOrEmailExists(request.Phone, request.Email, 0);
            if (isPatientExists != Constants.NOT_FOUND)
                return ApiResponse<string>.Conflict(isPatientExists);

            var mappedResult = _mapper.Map<Patients>(request);
            mappedResult.CreatedBy = Convert.ToInt64(tokenData.UserId);
            mappedResult.CreatedDate = DateTime.UtcNow;

            string response = await _patientRepository.CreatePatientAsync(mappedResult);
            if (response == Constants.SUCCESS)
                return ApiResponse<string>.Created(Constants.SUCCESS);

            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<string>> UpdatePatientAsync(PatientRequest request)
        {
            var tokenData = await _commonService.GetDataFromToken();
            var isPatientExists = await _patientRepository.PhoneNumberOrEmailExists(request.Phone, request.Email, request.Id);
            if (isPatientExists == Constants.SUCCESS)
                return ApiResponse<string>.Conflict(isPatientExists);
            var patientData = await _patientRepository.GetPatientByIdAsync(request.Id);
            if (patientData is null)
                return ApiResponse<string>.NotFound(Constants.ALLERGY_NOT_FOUND);

            var mappedResult = _mapper.Map<Patients>(request);
            mappedResult.UpdatedBy = Convert.ToInt64(tokenData.UserId);
            mappedResult.UpdatedDate = DateTime.UtcNow;

            string response = await _patientRepository.UpdatePatientAsync(mappedResult);
            if (response == Constants.SUCCESS)
                return ApiResponse<string>.Success(Constants.SUCCESS);

            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<string>> DeletePatientAsync(long patientId)
        {
            if (patientId <= 0)
                return ApiResponse<string>.BadRequest(Constants.BAD_REQUEST);

            var result = await _patientRepository.GetPatientByIdAsync(patientId);
            if (result is null)
                return ApiResponse<string>.NotFound(Constants.NOT_FOUND);

            var response = await _patientRepository.DeletePatientByIdAsync(patientId);
            if (response == Constants.SUCCESS)
                return ApiResponse<string>.Success(Constants.SUCCESS, response);

            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<string>> RegisterPatient(RegisterPatientRequest request)
        {
            var tokenData = await _commonService.GetDataFromToken();
            HttpClient _httpClient = new HttpClient();
            var patientData = await _patientRepository.GetPatientByPhoneNumber(request.PhoneNumber);
            if (patientData != Constants.NOT_FOUND)
                return ApiResponse<string>.Conflict(Constants.PHONE_ALREADY_EXISTS);
            string assistedCode = await _commonService.GenerateAssitedCode();

            var httpContext = _httpContextAccessor.HttpContext;
            var accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
                throw new UnauthorizedAccessException(Constants.USER_NOT_AUTHENTICATED);

            if (accessToken.StartsWith("bearer ")) accessToken = accessToken.Substring("bearer ".Length);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var authId = await _patientRepository.GetAuthIdByTypeAsync(Constants.EMAIL);
            Patients patients = new Patients();
            patients.Code = assistedCode;
            patients.Phone = request.PhoneNumber;
            patients.CreatedBy = Convert.ToInt64(tokenData.UserId);
            patients.CreatedDate = DateTime.UtcNow;
            patients.IsActive = false;

            var registeredPatient = await _patientRepository.RegisterPatientAsync(patients);
            if (registeredPatient is null)
                return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);

            var filePath = _configuration["Templates:PatientRegistration"];
            string subject = $"Your Otp Code For registration";
            string body = File.ReadAllText(filePath).Replace("[Registration Code]", $"{registeredPatient.Code}");
            var payload = new
            {
                To = "string88@yopmail.com",
                Subject = subject,
                Body = body,
                Name = "asgawg"
            };
            var jsonPayload = JsonConvert.SerializeObject(payload);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                $"{_configuration["Url"]}/SendGrid/SendMail",
                content
            );

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return ApiResponse<string>.Success(Constants.SUCCESS);
            }
            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<string>> VerifyPatientAsync(VerifyPatientRequest request)
        {
            var otpData = await _patientRepository.VerifyPatientAsync(request);
            if (otpData is null)
                return ApiResponse<string>.BadRequest(Constants.INVALID_OTP);

            var updatedPatientData = await _patientRepository.UpdatePatientStatus(request.PatientCode);

            if (updatedPatientData != Constants.SUCCESS)
                return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);

            var response = await _patientRepository.UpdateOtpDataAsync(otpData);
            if (response == Constants.SUCCESS)
                return ApiResponse<string>.Success(Constants.SUCCESS);

            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<string>> CreatePasswordAsync(CreatePasswordRequest request)
        {
            var hashedPassword = await _commonService.HashPassword(request.Password);
            request.Password = hashedPassword;
            bool isSuccess = await _patientRepository.CreatePasswordRequest(request);
            if (!isSuccess)
                return ApiResponse<string>.NotFound(Constants.PATIENT_NOT_EXISTS);
            return ApiResponse<string>.Success($"patient {Constants.UPDATED_SUCCESSFULLY}");
        }

        public async Task<ApiResponse<string>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var tokendata = await _commonService.GetDataFromToken();
            request.UserId = Convert.ToInt64(tokendata.UserId);

            var patientData = await _patientRepository.CheckPatientExists(tokendata.UserId);

            if (patientData == null)
                return ApiResponse<string>.NotFound(Constants.USER_NOT_FOUND);

            request.OldPassword = await _commonService.HashPassword(request.OldPassword);
            if (request.OldPassword != patientData.Password)
                return ApiResponse<string>.BadRequest(Constants.INVALID_OLD_PASSWORD);

            request.Password = await _commonService.HashPassword(request.Password);
            bool isPasswordUpdated = await _patientRepository.UpdatePasswordAsync(patientData.Id, request.Password);
            if (!isPasswordUpdated)
                return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
            return ApiResponse<string>.Success(Constants.SUCCESS);
        }

        public async Task<ApiResponse<string>> SendOtpAsync(PatientOtpRequest request)
        {
            HttpClient _httpClient = new HttpClient();
            var otpCode = await _commonService.GenerateOTP(6);
            var patientData = await _patientRepository.GetPatientByCodeAsync(request.PatientCode);
            if (patientData is null)
                return ApiResponse<string>.NotFound(Constants.NOT_FOUND);

            if (patientData.IsActive)
                return ApiResponse<string>.BadRequest(Constants.USER_ALREADY_VERIFIED);

            var filePath = _configuration["Templates:PatientRegistration"];
            string subject = $"Your Otp Code For registration";
            string body = File.ReadAllText(filePath);
            var authType = await _patientRepository.GetAuthIdByTypeAsync(Constants.EMAIL);
            var otpData = await _commonService.CreateOtpRequestAsync(patientData.Id.Value, "Patient Verification", authType);
            if (otpData is null)
                return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
            var payload = new
            {
                To = "string88@yopmail.com",
                Subject = subject,
                Body = body,
                Name = "asgawg"
            };
            var jsonPayload = JsonConvert.SerializeObject(payload);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                $"{_configuration["Url"]}/SendGrid/SendMail",
                content
            );

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return ApiResponse<string>.Success(Constants.SUCCESS);
            }
            return ApiResponse<string>.NotFound(Constants.NOT_FOUND);
        }
    }
}
