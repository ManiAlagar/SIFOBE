using SIFO.Model.Response;
using SIFO.Model.Constant;
using SIFO.Utility.Implementations;
using SIFO.APIService.Hospital.Service.Contracts;
using SIFO.APIService.Hospital.Repository.Contracts;

namespace SIFO.APIService.Hospital.Service.Implementations
{
    public class HospitalService : IHospitalService
    {
        private readonly IHospitalRepository _hospitalRepository;

        public HospitalService(IHospitalRepository hospitalRepository)
        {
            _hospitalRepository = hospitalRepository;
        }

        public async Task<ApiResponse<HospitalResponse>> GetHospitalByIdAsync(long hospitalId)
        {
            if (hospitalId <= 0)
                return ApiResponse<HospitalResponse>.BadRequest();

            var response = await _hospitalRepository.GetHospitalByIdAsync(hospitalId);

            if (response != null)
                return ApiResponse<HospitalResponse>.Success(Constants.SUCCESS, response);

            return ApiResponse<HospitalResponse>.NotFound();
        }

        public async Task<ApiResponse<string>> DeleteHospitalAsync(long hospitalId)
        {
            var response = await _hospitalRepository.DeleteHospitalAsync(hospitalId);
            if (response == Constants.NOT_FOUND)
                return new ApiResponse<string>(StatusCodes.Status404NotFound, Constants.HOSPITAL_NOT_FOUND);
            if (response == Constants.DATADEPENDENCYERRORMESSAGE)
                return new ApiResponse<string>(StatusCodes.Status400BadRequest, Constants.DATADEPENDENCYERRORMESSAGE);
            return ApiResponse<string>.Success(Constants.SUCCESS, response);
        }

        public async Task<ApiResponse<PagedResponse<HospitalResponse>>> GetAllHospitalAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection)
        {
            var isValid = await HelperService.ValidateGet(pageNo, pageSize, filter, sortColumn, sortDirection);

            if (isValid.Any())
                return ApiResponse<PagedResponse<HospitalResponse>>.BadRequest(isValid[0]);

            var response = await _hospitalRepository.GetAllHospitalAsync(pageNo, pageSize, filter, sortColumn, sortDirection);

            return ApiResponse<PagedResponse<HospitalResponse>>.Success(Constants.SUCCESS, response);
        }
    }
}
