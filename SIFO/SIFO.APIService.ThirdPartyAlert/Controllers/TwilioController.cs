using Microsoft.AspNetCore.Mvc;
using SIFO.Core.Service.Contracts;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.ThirdPartyAlert.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwilioController : ControllerBase
    {
        private readonly ITwilioService _twilioService;
        public TwilioController(ITwilioService twilioService)
        {
            _twilioService = twilioService;
        }

        [HttpPost]
        [Route("SendSms")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendSmsAsync(TwilioSendSmsRequest request)
        {
            try
            {
                var result = await _twilioService.SendSmsAsync(request);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }


        [HttpGet]
        [Route("Authy")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Send2FaAsync([FromHeader]long userId)
        {
            try
            {
                var result = await _twilioService.Send2FaAsync(userId);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                var result = ApiResponse<string>.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPost]
        [Route("Verify-Authy")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VerifySend2FaAsync([FromHeader] long userId, [FromHeader]string verifyCode, [FromHeader] string? pathId)
        {
            try
            {
                var result = await _twilioService.Verify2FaAsync(userId, verifyCode, pathId);
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
