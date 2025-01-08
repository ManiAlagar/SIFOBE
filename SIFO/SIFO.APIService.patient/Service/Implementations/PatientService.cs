using AutoMapper;
using Newtonsoft.Json;
using SIFO.APIService.Patient.Repository.Contracts;
using SIFO.APIService.Patient.Service.Contracts;
using SIFO.Common.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;
using SIFO.Utility.Implementations;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http.Headers;
using System.Text;

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

            var response = await _patientRepository.GetPatientAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll , tokenData.Role);
            return ApiResponse<PagedResponse<PatientResponse>>.Success(Constants.SUCCESS, response);
        }

        public async Task<ApiResponse<PatientResponse>> GetPatientByIdAsync(long patientId)
        {
            if (patientId <= 0)
                return ApiResponse<PatientResponse>.BadRequest();

            var response = await _patientRepository.GetPatientByIdAsync(patientId);

            if (response != null)
                return ApiResponse<PatientResponse>.Success(Constants.SUCCESS, response);

            return ApiResponse<PatientResponse>.NotFound();
        }
        public async Task<ApiResponse<string>> CreatePatientAsync(PatientRequest request)
        {
            var tokenData = await _commonService.GetDataFromToken();
            var isPatientExists = await _patientRepository.PhoneNumberOrEmailExists(request.Phone, request.Email,0);
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
            mappedResult.UpdatedDate= DateTime.UtcNow;

            string response = await _patientRepository.UpdatePatientAsync(mappedResult);
            if (response == Constants.SUCCESS)
                return ApiResponse<string>.Success(Constants.SUCCESS);

            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }
        public async Task<ApiResponse<string>> DeletePatientAsync(long patientId)
        {
            if (patientId <= 0)
                return ApiResponse<string>.BadRequest();

            var result = await _patientRepository.GetPatientByIdAsync(patientId);
            if (result is null)
                return ApiResponse<string>.NotFound();

            var response = await _patientRepository.DeletePatientByIdAsync(patientId);
            if (response == Constants.SUCCESS)
                return ApiResponse<string>.Success(Constants.SUCCESS, response);

            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<string>> RegisterPatient(RegisterPatientRequest request)
        {
            var tokenData = await _commonService.GetDataFromToken();
            HttpClient _httpClient = new HttpClient();
            var patientData= await _patientRepository.GetPatientByPhoneNumber(request.PhoneNumber);
            if (patientData != Constants.NOT_FOUND)
                return ApiResponse<string>.Conflict(Constants.PHONE_ALREADY_EXISTS);
            string assistedCode = string.Empty;
            string otp = await _commonService.GenerateOTP(6);
            while (true)
            {
                assistedCode = await _commonService.GenerateAssitedCode(14);
                var isExists = await _patientRepository.AssistedCodeExistsAsync(assistedCode); 
                if(!isExists) 
                    break;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            var accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
                throw new UnauthorizedAccessException("user not authenticated");

            if (accessToken.StartsWith("bearer "))  accessToken = accessToken.Substring("bearer ".Length);
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var authId = await _patientRepository.GetAuthIdByTypeAsync(Constants.EMAIL);
            Patients patients = new Patients();
            patients.Code = assistedCode;
            patients.Phone = request.PhoneNumber;
            patients.CreatedBy = Convert.ToInt64(tokenData.UserId);
            patients.CreatedDate = DateTime.UtcNow;  
            patients.IsActive = true;

            var registeredPatient = await _patientRepository.RegisterPatientAsync(patients); 
            if(registeredPatient is null) 
                return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);

            var otpData = await _commonService.CreateOtpRequestAsync(patients.Id.Value , "patient registration" , authId); 
            if(otpData is null)
                return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
            var filePath = _configuration["Templates:PatientRegistration"];
            string subject = $"Your Otp Code For registration";
            string body = File.ReadAllText(filePath).Replace("[OTP Code]", otpData.OtpCode).Replace("[X]", _configuration["OtpExpiration"]).Replace("[Verification Link]", $"{_configuration["VerificationLink"]}/{registeredPatient.Code}");
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
            }
            return ApiResponse<string>.Success();
        }

        public async Task<ApiResponse<string>> VerifyPatientAsync(VerifyPatientRequest request)
        { 
            var otpData = await _patientRepository.VerifyPatientAsync(request);
            if (otpData is null)
                return ApiResponse<string>.BadRequest(Constants.INVALID_OTP);

            var response =  await _patientRepository.UpdateOtpDataAsync(otpData);
            if (response == Constants.SUCCESS)
                return ApiResponse<string>.Success();

            return ApiResponse<string>.InternalServerError();
        }

        public async Task<ApiResponse<string>> CreatePasswordAsync(CreatePasswordRequest request)
        { 
            var hashedPassword = await _commonService.HashPassword(request.Password);
            request.Password = hashedPassword;
            bool isSuccess = await _patientRepository.CreatePasswordRequest(request);
            if (!isSuccess)
                return ApiResponse<string>.NotFound("Patient not exists");
            return ApiResponse<string>.Success($"patient {Constants.UPDATED_SUCCESSFULLY}");
        }
        public async Task<ApiResponse<string>> ChangePasswordAsync(ChangePasswordRequest request)
        {
           var tokendata = await _commonService.GetDataFromToken();
            request.UserId = Convert.ToInt64(tokendata.UserId);

            var patientData = await  _patientRepository.CheckPatientExists(tokendata.UserId);

            if (patientData == null)
                return ApiResponse<string>.NotFound("user not found");

            request.OldPassword = await _commonService.HashPassword(request.OldPassword);
            if (request.OldPassword != patientData.Password)
                return ApiResponse<string>.BadRequest("invalid old password");

            request.Password = await _commonService.HashPassword(request.Password);
            bool isPasswordUpdated = await _patientRepository.UpdatePasswordAsync(patientData.Id, request.Password);
            if (!isPasswordUpdated)
                return ApiResponse<string>.InternalServerError();
            return ApiResponse<string>.Success();
        }
    }
    }
