using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.ThirdPartyAlert.ThirdPartyService.Contracts
{
    public interface IThirdPartyService
    {
        public Task<bool> SendMail(List<string> to, List<string>? cc, string subject, string body);
        public Task<bool> SendSms(List<string> phoneNumbers, string body);
        public Task<ApiResponse<string>> SendMailAsync(string toMail, string subject, string body, string name);
        public Task<ApiResponse<object>> SendSmsAsync(TwilioSendSmsRequest request);
        public Task<ApiResponse<object>> Send2FaAsync(long userId);
         public Task<ApiResponse<string>> Verify2FaAsync(long userId, string verifyCode, string pathId);
         public Task<string> SendOtpRequestAsync(long userId, string authenticationFor, long authenticationType);
    }
}
