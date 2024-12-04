using SIFO.APIService.Master.Repository.Contracts;
using SIFO.APIService.Master.Service.Contracts;
using SIFO.Common.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Master.Service.Implementations
{
    public class MasterService : IMasterService
    { 
        private readonly IMasterRepository _masterRepository ;
        private readonly ICommonService _commonService; 
        private readonly IConfiguration _configuration;
        public MasterService(IConfiguration configuration,IMasterRepository masterRepository,ICommonService commonService)
        {
            _masterRepository = masterRepository; 
            _configuration = configuration; 
            _commonService = commonService;
        }
        public async Task<ApiResponse<string>> SendOtpRequestAsync(SendOtpRequest request)
        {

            var userData = await _masterRepository.IsUserExists(request.UserId); 
            if(userData is null)
                return ApiResponse<string>.NotFound(Constants.NOT_FOUND);
            var authType = await _commonService.GetAuthenticationTypeByIdAsync(request.AuthenticationType);
            if (authType is null)
                return ApiResponse<string>.NotFound(Constants.NOT_FOUND);
            var otpData = await _commonService.CreateOtpRequestAsync(request.UserId, request.AuthenticationFor, request.AuthenticationType);

            var filePath = authType.AuthType.ToLower() == Constants.EMAIL ? _configuration["Templates:Email"] : _configuration["Templates:Sms"];
            string subject = $"Your Otp Code For {request.AuthenticationFor}";
            string body = File.ReadAllText(filePath).Replace("[UserName]", $"{userData.FirstName} {userData.LastName}").Replace("[OTP Code]", otpData.OtpCode).Replace("[X]", _configuration["OtpExpiration"]).Replace("[EventName]", request.AuthenticationFor);
            if (authType.AuthType.ToLower() == Constants.EMAIL)
            {
                string[] mail = new string[] { userData.Email };
                bool isMailSent = await _commonService.SendMail(mail.ToList(), null, subject, body);
                if (!isMailSent)
                    return ApiResponse<string>.InternalServerError("something went wrong while sending the mail");
                return ApiResponse<string>.Success("Otp send Successfully");
            }
            else if (authType.AuthType.ToLower() == Constants.SMS)
            {
                string[] phoneNumbers = new string[] { userData.PhoneNumber };
                bool isSmsSent = await _commonService.SendSms(phoneNumbers.ToList(), body);
                if (!isSmsSent)
                    return ApiResponse<string>.InternalServerError("something went wrong while sending the sms");
                return ApiResponse<string>.Success("Otp send Successfully");
            }
            return ApiResponse<string>.InternalServerError();
        }
    }
}
