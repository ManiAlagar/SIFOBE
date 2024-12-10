using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SIFO.Model.Response;
using System.Net;

namespace SIFO.Core.MiddleWare
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Pass to the next middleware
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            // Customize exception handling based on the type of exception
            HttpStatusCode statusCode;
            string message;

            switch (exception)
            {
                case ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = "Unauthorized access.";
                    break;

                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = "Resource not found.";
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred.";
                    break;
            }

            response.StatusCode = (int)statusCode;

            var result = JsonConvert.SerializeObject(ApiResponse<string>.InternalServerError(

                "Message : " + exception.Message + " InnerException : " + exception.InnerException
            ));

            return response.WriteAsync(result);
        }
    }
}
