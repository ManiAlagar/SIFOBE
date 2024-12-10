using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.Core.Service.Contracts
{
    public interface ISendGridService
    {
        public Task<ApiResponse<string>> SendMailAsync(string toMail,string subject , string body, string name);
    }
}
