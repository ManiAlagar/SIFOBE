using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SIFO.Model.Entity;
using SIFO.Model.Response;
using System.Text.Json.Nodes;

public class OtpValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<OtpValidationMiddleware> _logger;

    public OtpValidationMiddleware(RequestDelegate next, ILogger<OtpValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {

            var apiName = context.Request.Path.ToString();
            var apiData = apiName.Split("/");
            var methodType = context.Request.Method;

            var authenticationType = context.Request.Headers["AuthenticationType"].ToString();
            long authenticationTypeId = default;
            if (!string.IsNullOrEmpty(authenticationType))
                authenticationTypeId = Convert.ToInt64(authenticationType);

            var authenticationFor = context.Request.Headers["AuthenticationFor"].ToString(); 
            var otpCode = context.Request.Headers["OtpCode"].ToString(); 
            var user = context.Request.Headers["UserId"].ToString();

            long userId = default;
            if (!string.IsNullOrEmpty(user))
                userId = Convert.ToInt64(user);

            string requestBody = string.Empty;
            if (context.Request.ContentLength > 0 && context.Request.ContentType?.Contains("application/json") == true)
            {
                context.Request.EnableBuffering();
                using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
                {
                    requestBody = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }
            }

            using (var scope = context.RequestServices.CreateScope())
            {

                var dbContext = scope.ServiceProvider.GetRequiredService<SIFOContext>();
                if (methodType.ToLower() != "put" && !apiName.ToLower().Contains("verify-login"))
                {
                    await _next(context);
                    return;
                }
                if(string.IsNullOrEmpty(otpCode) || userId <= 0 || string.IsNullOrEmpty(authenticationFor) || string.IsNullOrEmpty(authenticationType))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(ApiResponse<string>.BadRequest("Please add required header fields")));
                    return;
                }

                var otpData = await dbContext.OtpRequests.OrderByDescending(a => a.Id).Where(a => a.OtpCode == otpCode
                                && a.AuthenticationFor.ToLower() == authenticationFor.ToLower() && a.AuthenticationType == authenticationTypeId
                                && a.UserId == userId
                                && a.ExpirationDate > DateTime.UtcNow && a.VerifiedDate == null).FirstOrDefaultAsync(); 

                if (otpData is null)
                { 
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(ApiResponse<string>.Forbidden("invalid OTP")));
                    return;
                } 
                await _next(context);
                if (context.Response.StatusCode == 200)
                {
                    otpData.VerifiedDate = DateTime.UtcNow;
                    otpData.UpdatedDate = DateTime.UtcNow;
                    otpData.UpdatedBy = userId;
                    await dbContext.SaveChangesAsync();
                }
            }
        }
        catch (Exception ex)
        {
            var details = ApiResponse<string>.InternalServerError(ex.Message);
            var response = JsonConvert.SerializeObject(details);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(response);
        }
    }
}
