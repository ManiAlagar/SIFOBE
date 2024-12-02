using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SIFO.Model.Response;

namespace SIFO.APIService.Authentication.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error()
        {
            Exception exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

            var result = ApiResponse<string>.InternalServerError(exception.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }
}
