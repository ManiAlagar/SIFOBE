using Grpc.Core;
using SIFO.APIService.ThirdParty;
using SIFO.APIService.ThirdPartyAlert.Repository.Contracts;
using SIFO.APIService.ThirdPartyAlert.Repository.Implementations;
using SIFO.APIService.ThirdPartyAlert.ThirdPartyService.Contracts;
using SIFO.APIService.ThirdPartyAlert.ThirdPartyService.Implementations;
using SIFO.Model.Constant;

public class ThirdPartyServiceImpl : ThirdPartyGrpcService.ThirdPartyGrpcServiceBase
{
    private readonly IThirdPartyService _thirdPartyService;
    private readonly IThirdPartyRepository _thirdPartyRepository;
    public ThirdPartyServiceImpl(IThirdPartyService thirdPartyService, IThirdPartyRepository thirdPartyRepository)
    {
        _thirdPartyService = thirdPartyService;
        _thirdPartyRepository = thirdPartyRepository;
    }
    public override async Task<SendMailResponse> SendMail(SendMailRequest request, ServerCallContext context)
    {
        try
        {
            bool emailSent = await _thirdPartyService.SendMail(request.To.ToList(), request.Cc.ToList(), request.Subject, request.Body);
            return new SendMailResponse { Success = emailSent };
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while sending the email."));
        }
    }
    public async Task<string> Verify2FaAsync(long userId, string otpCode, string serviceId)
    {
        var response = await _thirdPartyService.Verify2FaAsync(userId, otpCode, serviceId);
        return response.Data;
    }
    public override async Task<IsValidAuthResponse> IsValidAuth(IsValidAuthRequest request, ServerCallContext context)
    {
        string serviceId = await _thirdPartyRepository.GetServiceIdbyUserIDAsync(request.UserId);
        var response = await _thirdPartyService.Verify2FaAsync(request.UserId, request.OtpCode, serviceId);
        return new IsValidAuthResponse
        {
            IsValid = response.Data == Constants.SUCCESS,
            Message = response.Message
        };
        }
}

