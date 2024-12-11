using Microsoft.AspNetCore.Mvc;
using SIFO.Core.Service.Contracts;
using SIFO.Model.Request;
using SIFO.Model.Response;
namespace SIFO.APIService.ThirdPartyAlert.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendGridController : ControllerBase
    { 
        private readonly ISendGridService _sendGridService;
        public SendGridController(ISendGridService sendGridService)
        {
            _sendGridService = sendGridService;
        }

        [HttpPost]
        [Route("SendMail")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendMail(SendGridMailRequest request)
        {
            try
            {
                var result = await _sendGridService.SendMailAsync(request.To,request.Subject,request.Body,request.Name);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
    }
}
