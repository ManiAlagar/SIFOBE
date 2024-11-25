﻿using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SIFO.AuthenticationService.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error()
        {
            Exception exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

            return StatusCode(500, "Internal Server Error");
        }
    }
}
