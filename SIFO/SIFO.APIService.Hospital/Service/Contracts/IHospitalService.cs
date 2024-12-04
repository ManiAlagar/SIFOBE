using Microsoft.AspNetCore.Mvc.ApiExplorer;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Hospital.Service.Contracts
{
    public interface IHospitalService
    {
        Task<ApiResponse<string>> CreateHospitalAsync(HospitalRequest request);
        Task<ApiResponse<string>> UpdateHospitalAsync(HospitalRequest request, long id);
    }
}
