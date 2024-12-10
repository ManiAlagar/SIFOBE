using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.Core.Service.Contracts
{
    public interface ITwilioService
    { 
        public Task<ApiResponse<object>> SendSmsAsync(TwilioSendSmsRequest request);
        public Task<ApiResponse<object>> Send2FaAsync(long userId);
        public Task<ApiResponse<string>> Verify2FaAsync(long userId, string verifyCode, string pathId);
    }
}
