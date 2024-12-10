using Twilio;
using Twilio.Types;
using Newtonsoft.Json;
using SIFO.Model.Request;
using SIFO.Model.Response;
using SIFO.Model.Constant;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service.Entity;
using Microsoft.IdentityModel.Tokens;
using SIFO.Model.Entity;
using SIFO.Core.Service.Contracts;
using Microsoft.Extensions.Configuration;
using SIFO.Core.Repository.Contracts;

namespace SIFO.Core.Service.Implementations
{ 
    public class TwilioService : ITwilioService
    {
        private readonly IConfiguration _configuration;
        private readonly ITwilioRepository _twilioRepository;

        public TwilioService(IConfiguration configuration, ITwilioRepository twilioRepository)
        {
            _configuration = configuration;
            _twilioRepository = twilioRepository;
        }

        public async Task<ApiResponse<object>> Send2FaAsync(long userId)
        {
            try
            {
                string accountSid = _configuration["TwilioSettings:AccountSid"];
                string authToken = _configuration["TwilioSettings:AuthToken"];
                string serviceSid = _configuration["TwilioSettings:ServiceSid"];
                string registeredMobNum = _configuration["TwilioSettings:RegisteredMobNum"];
                string accountName = _configuration["TwilioSettings:AccountName"];

                TwilioClient.Init(accountSid, authToken);

                var newFactor = await NewFactorResource.CreateAsync(
                    friendlyName: accountName,
                    factorType: NewFactorResource.FactorTypesEnum.Totp,
                    pathServiceSid: serviceSid,
                    pathIdentity: registeredMobNum);

                var result = JsonConvert.SerializeObject(newFactor.Binding);

                var response = new TotpResponse
                {
                    Message = "success",
                    Sid = newFactor.Sid,
                    bindingsResponse = JsonConvert.DeserializeObject<BindingsResponse>(result)
                };

                await _twilioRepository.CreateOrUpdateServiceIdAsync(userId, newFactor.Sid);

                return ApiResponse<object>.Success(Constants.SUCCESS, response);
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.InternalServerError(ex.Message);
            }
        }

        public async Task<ApiResponse<object>> SendSmsAsync(TwilioSendSmsRequest request)
        {
            var accountSid = _configuration["TwilioSettings:AccountSid"];
            var authToken = _configuration["TwilioSettings:AuthToken"];
            var from = _configuration["TwilioSettings:from"];
            TwilioClient.Init(accountSid, authToken);
            try
            {
                var message = await MessageResource.CreateAsync(body: request.Body,from: new PhoneNumber(from),to: new PhoneNumber(request.To));
                var smsResponse = new
                {
                    message.Sid,
                    message.Status,
                    message.Body,
                    message.To,
                    message.From,
                    message.DateSent
                };
                if (message.Status == MessageResource.StatusEnum.Queued || message.Status == MessageResource.StatusEnum.Sent)
                    return ApiResponse<object>.Success(Constants.SUCCESS , smsResponse);

                return ApiResponse<object>.BadRequest($"{message.Status}");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.InternalServerError(ex.Message);
            }
        }

        public async Task<ApiResponse<string>> Verify2FaAsync(long userId, string verifyCode ,string? pathId)
        {
            try
            {
                string accountSid = _configuration["TwilioSettings:AccountSid"];
                string authToken = _configuration["TwilioSettings:AuthToken"];
                string serviceSid = _configuration["TwilioSettings:ServiceSid"];
                string registeredMobNum = _configuration["TwilioSettings:RegisteredMobNum"];

                TwilioClient.Init(accountSid, authToken);

                if (!string.IsNullOrEmpty(pathId))
                    pathId = await _twilioRepository.GetServiceIdbyUserIDAsync(userId);

                await _twilioRepository.CreateOrUpdateServiceIdAsync(userId, pathId);

                var factor = await FactorResource.UpdateAsync(
                    authPayload: verifyCode,
                    pathServiceSid: serviceSid,
                    pathIdentity: registeredMobNum,
                    pathSid: pathId);

                if (factor.Status == FactorResource.FactorStatusesEnum.Verified)
                {
                    var response = await ChallengeResource.CreateAsync(
                      authPayload: verifyCode,
                      factorSid: pathId,
                      pathServiceSid: serviceSid,
                      pathIdentity: registeredMobNum);
                    if (response.Status.ToString().ToLower() == "approved")
                        return ApiResponse<string>.Success(Constants.SUCCESS);
                    return ApiResponse<string>.BadRequest("otp expired");
                }
                else
                    return ApiResponse<string>.InternalServerError();
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.InternalServerError(ex.Message);
            }
        }
    }
}
