using SIFO.Model.Response;

namespace SIFO.APIService.Hospital.Repository.Contracts
{
    public interface IHospitalRepository
    {
        public Task<PagedResponse<HospitalResponse>> GetAllHospitalAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection);
        public Task<HospitalResponse> GetHospitalByIdAsync(long hospitalId);
        public Task<string> DeleteHospitalAsync(long hospitalId);
    }
}
