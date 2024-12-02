using SIFO.Model.Entity;

namespace SIFO.APIService.Master.Repository.Contracts
{
    public interface IMasterRepository
    {
        public Task<Users> IsUserExists(long userId);
        public Task<Users> GetUserByEmail(string email);
    }
}
