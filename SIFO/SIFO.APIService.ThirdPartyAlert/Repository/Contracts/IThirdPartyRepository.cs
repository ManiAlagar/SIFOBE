namespace SIFO.APIService.ThirdPartyAlert.Repository.Contracts
{
    public interface IThirdPartyRepository
    {
     //  public Task<string> SendOtpRequestAsync(long userId, string authenticationFor, long authenticationType);
        public Task<string?> GetServiceIdbyUserIDAsync(long userId);
        public Task<bool> CreateOrUpdateServiceIdAsync(long userId, string userSid);
    }
}
