using OfficeOpenXml;
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

        public async Task<ApiResponse<LabelResponse>> GetLabelsAsync()
        {
            var result = await _masterRepository.GetLabelsAsync();
            return ApiResponse<LabelResponse>.Success("", result);
        }

        public async Task<ApiResponse<string>> ImportLableAsync(LabelRequest request)
        {
            if (string.IsNullOrEmpty(request.FilePath))
                return ApiResponse<string>.BadRequest("File path is null or empty.");
            if (!System.IO.File.Exists(request.FilePath))
                return ApiResponse<string>.BadRequest($"File does not exist at the path: {request.FilePath}");

            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            List<Labels> lables = new List<Labels>();
            using (var package = new ExcelPackage(new FileInfo(request.FilePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;
                for (int row = 2; row <= rowCount; row++)
                {
                    var variable = worksheet.Cells[row, 1].Text;
                    var textEn = worksheet.Cells[row, 2].Text;
                    var textIt = worksheet.Cells[row, 3].Text;

                    var englishData = new Labels
                    {
                        FkVar = variable,
                        Language = "EN",
                        Label = textEn
                    };

                    var italyData = new Labels
                    {
                        FkVar = variable,
                        Language = "IT",
                        Label = textIt
                    };
                    lables.Add(englishData);
                    lables.Add(italyData);
                }
            }
            var result = await _masterRepository.ImportLableAsync(lables);
            if (result == Constants.SUCCESS)
                return ApiResponse<string>.Success();
            return ApiResponse<string>.InternalServerError();
        }

    }
}
