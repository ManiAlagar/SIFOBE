namespace SIFO.Core.Repository.Contracts
{
    public interface ITwilioRepository
    {
        public Task<string?> GetServiceIdbyUserIDAsync(long userId);
        public Task<bool> CreateOrUpdateServiceIdAsync(long userId, string userSid);
    }
}
