using SIFO.Model.Entity;
using SIFO.Model.Response;

namespace SIFO.APIService.Patient.Repository.Contracts
{
    public interface IAllergyRepository
    {
        public Task<PagedResponse<AllergyResponse>> GetAllAllergyAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, long patientId);
        public Task<AllergyResponse> GetAllergyByIdAsync(long allergyId);
        public Task<bool> CreateAllergyAsync(Allergy entity);
        public Task<bool> UpdateAllergyAsync(Allergy entity, long allergyId);
        public Task<string> DeleteAllergyAsync(long allergyId);
        //public Task<Allergy> AllergyNameExistsAsync(string? name, long? allergyId, long? patientId);
    }
}
