using SIFO.Model.Request;

namespace SIFO.APIService.Hospital.Repository.Contracts
{
    public interface IHospitalRepository
    {
        Task<bool> SaveHospitalAsync(HospitalRequest request);
        Task<bool> CheckIfEmailOrPhoneExists(string phoneNumber,long userID);
        Task<bool> UpdateHospitalAsync(HospitalRequest request,long id);

    }
}
