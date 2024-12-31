using System.Net.Mail;
using Twilio.Types;
using Twilio;
using SIFO.APIService.ThirdPartyAlert.ThirdPartyService.Contracts;
using Twilio.Rest.Api.V2010.Account;
using SendGrid.Helpers.Mail;
using SIFO.Model.Response;
using SendGrid;
using Newtonsoft.Json;
using SIFO.Model.Request;
using Twilio.Rest.Verify.V2.Service.Entity;
using SIFO.Model.Constant;
using SIFO.APIService.ThirdPartyAlert.Repository.Contracts;
using SIFO.APIService.ThirdPartyAlert.Repository.Implementations;
using Microsoft.EntityFrameworkCore;
using SIFO.Model.Entity;
using SIFO.Common.Contracts;

namespace SIFO.APIService.ThirdPartyAlert.ThirdPartyService.Implementations
{
    public class ThirdPartyService : IThirdPartyService
    {
        private readonly IConfiguration _configuration;
        private readonly ISendGridClient _sendGridClient;
        private readonly IThirdPartyRepository _thirdPartyRepository;
        private readonly ICommonService _commmonService;
        private readonly SIFOContext _context;

        public ThirdPartyService(IConfiguration configuration, IThirdPartyRepository thirdPartyRepository,SIFOContext context,ICommonService commonService)
        {
            _configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            _thirdPartyRepository = thirdPartyRepository;
            _context = context; 
            _commmonService = commonService;    
        }

        public async Task<bool> SendMail(List<string> to, List<string>? cc, string subject, string body) // need to use send grid mail here 
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_configuration["MailCredentials:Mail"]);
            foreach (var userMail in to)
            {
                mail.To.Add(new MailAddress(userMail));
            }
            if (cc?.Count > 0)
            {
                foreach (string ccmail in cc)
                {
                    mail.CC.Add(new MailAddress(ccmail));
                }
            }
            mail.Body = body;
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.Credentials = new System.Net.NetworkCredential(_configuration["MailCredentials:Mail"], _configuration["MailCredentials:Password"]);
            smtpClient.EnableSsl = true;
            try
            {
                smtpClient.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        public async Task<bool> SendSms(List<string> phoneNumbers, string body)
        {
            try
            {
                string accountSid = _configuration["Twilio:AccountSid"];
                string authToken = _configuration["Twilio:AuthToken"];
                string fromPhoneNumber = _configuration["Twilio:PhoneNumber"];
                TwilioClient.Init(accountSid, authToken);
                foreach (var phoneNumber in phoneNumbers)
                {
                    var toPhoneNumber = new PhoneNumber(phoneNumber);
                    var message = await MessageResource.CreateAsync(
                       to: toPhoneNumber,
                       from: new PhoneNumber(fromPhoneNumber),
                       body: body
                   );
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<ApiResponse<string>> SendMailAsync(string toMail, string subject, string body, string name)
        {
            var fromEmail = new EmailAddress(_configuration["SendGridSettings:From"], "SIFO");
            var toEmail = new EmailAddress(toMail, name);
            var htmlContent = body;
            var mailMsg = MailHelper.CreateSingleEmail(fromEmail, toEmail, subject, null, htmlContent);
            try
            {
                var response = await _sendGridClient.SendEmailAsync(mailMsg);
                if (response.IsSuccessStatusCode)
                    return ApiResponse<string>.Success(Constants.SUCCESS);

                return ApiResponse<string>.InternalServerError();
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.InternalServerError(ex.Message);
            }
        }
        public async Task<ApiResponse<object>> Send2FaAsync(long userId)
        {
            try
            {
                string accountSid = _configuration["TwilioSettings:AccountSid"];
                string authToken = _configuration["TwilioSettings:AuthToken"];
                string serviceSid = _configuration["TwilioSettings:ServiceSid"];
                string registeredMobNum = _configuration["TwilioSettings:RegisteredMobNum"];
                string accountName = _configuration["TwilioSettings:AccountName"];

                TwilioClient.Init(accountSid, authToken);

                var newFactor = await NewFactorResource.CreateAsync(
                    friendlyName: accountName,
                    factorType: NewFactorResource.FactorTypesEnum.Totp,
                    pathServiceSid: serviceSid,
                    pathIdentity: registeredMobNum);

                var result = JsonConvert.SerializeObject(newFactor.Binding);

                var response = new TotpResponse
                {
                    Message = "success",
                    Sid = newFactor.Sid,
                    bindingsResponse = JsonConvert.DeserializeObject<BindingsResponse>(result)
                };

                await _thirdPartyRepository.CreateOrUpdateServiceIdAsync(userId, newFactor.Sid);

                return ApiResponse<object>.Success(Constants.SUCCESS, response);
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.InternalServerError(ex.Message);
            }
        }

        public async Task<ApiResponse<object>> SendSmsAsync(TwilioSendSmsRequest request)
        {
            var accountSid = _configuration["TwilioSettings:AccountSid"];
            var authToken = _configuration["TwilioSettings:AuthToken"];
            var from = _configuration["TwilioSettings:from"];
            TwilioClient.Init(accountSid, authToken);
            try
            {
                var message = await MessageResource.CreateAsync(body: request.Body, from: new PhoneNumber(from), to: new PhoneNumber(request.To));
                var smsResponse = new
                {
                    message.Sid,
                    message.Status,
                    message.Body,
                    message.To,
                    message.From,
                    message.DateSent
                };
                if (message.Status == MessageResource.StatusEnum.Queued || message.Status == MessageResource.StatusEnum.Sent)
                    return ApiResponse<object>.Success(Constants.SUCCESS, smsResponse);

                return ApiResponse<object>.BadRequest($"{message.Status}");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.InternalServerError(ex.Message);
            }
        }
        public async Task<string> SendOtpRequestAsync(long userId, string authenticationFor, long authenticationType)
        {
            var userData = await _context.Users.Where(a => a.Id == userId).SingleOrDefaultAsync();

            var authType = await _commmonService.GetAuthenticationTypeByIdAsync(authenticationType);
            var otpData = await _commmonService. CreateOtpRequestAsync(userId, authenticationFor, authenticationType);

            var filePath = authType.AuthType.ToLower() == Constants.EMAIL ? _configuration["Templates:Email"] : _configuration["Templates:Sms"];
            string subject = $"Your Otp Code For {authenticationFor}";
            string body = File.ReadAllText(filePath).Replace("[UserName]", $"{userData.FirstName} {userData.LastName}").Replace("[OTP Code]", otpData.OtpCode).Replace("[X]", _configuration["OtpExpiration"]).Replace("[EventName]", authenticationFor);
            if (authType.AuthType.ToLower() == Constants.EMAIL)
            {
                //var mailResponse = await _sendGridService.SendMailAsync(userData.Email, subject, body, $"{userData.FirstName} {userData.LastName}");
                var toUser = new string[] { userData.Email };
                var mailResponse = await SendMail(toUser.ToList(), null, subject, body);
                //if (!mailResponse.IsSuccess) 
                if (!mailResponse)
                    return Constants.INTERNAL_SERVER_ERROR;
                return Constants.SUCCESS;
            }
            else if (authType.AuthType.ToLower() == Constants.SMS)
            {
                TwilioSendSmsRequest request = new TwilioSendSmsRequest();
                request.Body = body;
                request.To = userData.PhoneNumber;
                var isSmsSent = await SendSmsAsync(request);
                if (!isSmsSent.IsSuccess)
                    return Constants.INTERNAL_SERVER_ERROR;
                return Constants.SUCCESS;
            }
            else if (authType.AuthType.ToLower() == Constants.TWILIO_AUTHY)
            {
                TwilioSendSmsRequest request = new TwilioSendSmsRequest();
                request.Body = body;
                request.To = userData.PhoneNumber;
                var isSent = await Send2FaAsync(userId);
                if (!isSent.IsSuccess)
                    return Constants.INTERNAL_SERVER_ERROR;
                return Constants.SUCCESS;
            }
            return Constants.INTERNAL_SERVER_ERROR;
        }

        public async Task<ApiResponse<string>> Verify2FaAsync(long userId, string verifyCode, string? pathId)
        {
            try
            {
                string accountSid = _configuration["TwilioSettings:AccountSid"];
                string authToken = _configuration["TwilioSettings:AuthToken"];
                string serviceSid = _configuration["TwilioSettings:ServiceSid"];
                string registeredMobNum = _configuration["TwilioSettings:RegisteredMobNum"];

                TwilioClient.Init(accountSid, authToken);

                if (!string.IsNullOrEmpty(pathId))
                    pathId = await _thirdPartyRepository.GetServiceIdbyUserIDAsync(userId);

                await _thirdPartyRepository.CreateOrUpdateServiceIdAsync(userId, pathId);

                var factor = await FactorResource.UpdateAsync(
                    authPayload: verifyCode,
                    pathServiceSid: serviceSid,
                    pathIdentity: registeredMobNum,
                    pathSid: pathId);

                if (factor.Status == FactorResource.FactorStatusesEnum.Verified)
                {
                    var response = await ChallengeResource.CreateAsync(
                      authPayload: verifyCode,
                      factorSid: pathId,
                      pathServiceSid: serviceSid,
                      pathIdentity: registeredMobNum);
                    if (response.Status.ToString().ToLower() == "approved")
                        return ApiResponse<string>.Success(Constants.SUCCESS);
                    return ApiResponse<string>.BadRequest("otp expired");
                }
                else
                    return ApiResponse<string>.InternalServerError();
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.InternalServerError(ex.Message);
            }
        }

        public async Task<bool> IsValidAuth(long userId, string otpcode, HttpContext context)
        {
            using (var scope = context.RequestServices.CreateScope())
            {
                var _twilioRepository = scope.ServiceProvider.GetRequiredService<IThirdPartyRepository>();
                var _twilioService = scope.ServiceProvider.GetRequiredService<IThirdPartyService>();

                string serviceId = await _twilioRepository.GetServiceIdbyUserIDAsync(userId);
                var res = await _twilioService.Verify2FaAsync(userId, otpcode, serviceId);

                if (res.Message == Constants.SUCCESS)
                    return true;
                return false;
            }
        }
    }
}
