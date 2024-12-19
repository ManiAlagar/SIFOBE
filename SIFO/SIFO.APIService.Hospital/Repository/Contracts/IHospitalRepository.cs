using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Hospital.Repository.Contracts
{
    public interface IHospitalRepository
    {
        //public Task<PagedResponse<HospitalResponse>> GetAllHospitalAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection);
        //public Task<HospitalResponse> GetHospitalByIdAsync(long hospitalId);
        //public Task<string> DeleteHospitalAsync(long hospitalId);
        //public Task<bool> CreateHospitalAsync(HospitalRequest request);
        //public Task<bool> UpdateHospitalAsync(HospitalRequest request,long id);
        //public Task<bool> CheckIfEmailOrPhoneExists(string phoneNumber,long userID);
        public Task<bool> CalendarExistsAsync(long id);
        public Task<Dictionary<string, List<CalendarResponse>>> GetCalendarByIdAsync(long pharmacyId, DateTime startDate, DateTime endDate);
        public Task<string> CreateCalendarAsync(Calendar request);
        public Task<string> UpdateCalendarAsync(Calendar request);
        public Task<bool> GetPharmacyByIdAsync(long id);
    }
}
