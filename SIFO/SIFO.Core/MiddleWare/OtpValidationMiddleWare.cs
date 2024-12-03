using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SIFO.Model.Constant;
using SIFO.Model.Entity;
using SIFO.Model.Request;
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
                var data = JsonConvert.DeserializeObject<VerifyOtpRequest>(requestBody);
                if (methodType.ToLower() != "put" && authenticationFor.ToLower() != "login")
                {
                    await _next(context);
                    return;
                }
                if(string.IsNullOrEmpty(otpCode) || data.UserId<=0)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(ApiResponse<string>.BadRequest()));
                    return;
                }

                var otpData = await dbContext.OtpRequests.OrderByDescending(a => a.Id).Where(a => a.OtpCode == otpCode
                                && a.AuthenticationFor.ToLower() == authenticationFor && a.AuthenticationType == authenticationTypeId
                                && a.UserId == data.UserId 
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
                    otpData.UpdatedBy = data.UserId;
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

    private Dictionary<string, string>? ExtractPayloadAsJsonObject(string body)
    {
        try
        {
            var json = JsonConvert.DeserializeObject<JsonObject>(body);

            if (json == null)
                return null;

            var result = new Dictionary<string, string>();

            if (json.TryGetPropertyValue("authenticatedType", out JsonNode authenticatedTypeNode))
                result["authenticatedType"] = authenticatedTypeNode.GetValue<string>();

            if (json.TryGetPropertyValue("eventName", out JsonNode eventNameNode))
                result["eventName"] = eventNameNode.GetValue<string>();

            if (json.TryGetPropertyValue("otp", out JsonNode otpNode))
                result["otp"] = otpNode.GetValue<string>();

            if (json.TryGetPropertyValue("userId", out JsonNode userIdNode))
                result["userId"] = userIdNode.GetValue<string>();

            return result;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    //private async Task<OtpRequest> GetOtpDetailsAsync(long? userId,string OtpCode, string AuthenticationFor, long AuthenticationType)
    //{
    //    using (var scope = context.RequestServices.CreateScope())
    //    {
    //        try
    //        {
    //            var result = 
    //            return result;
    //        }
    //        catch (Exception ex)
    //        {
    //            throw;
    //        }
    //    }
    //}
}
