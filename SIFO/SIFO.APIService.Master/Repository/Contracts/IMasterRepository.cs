using SIFO.Model.Entity;
using SIFO.Model.Response;

namespace SIFO.APIService.Master.Repository.Contracts
{
    public interface IMasterRepository
    {
        public Task<Users> IsUserExists(long userId);
        public Task<string> ImportLableAsync(List<Labels> labels);
        public Task<LabelResponse> GetLabelsAsync();
    }
}
