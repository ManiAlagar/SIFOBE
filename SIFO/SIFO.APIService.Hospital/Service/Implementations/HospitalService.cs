using SIFO.Model.Response;
using SIFO.Model.Constant;
using SIFO.Utility.Implementations;
using SIFO.APIService.Hospital.Service.Contracts;
using SIFO.APIService.Hospital.Repository.Contracts;
using SIFO.Model.Request;

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

        public async Task<ApiResponse<string>> CreateHospitalAsync(HospitalRequest request)
        {
            try
            {
                bool isSuccess = await _hospitalRepository.SaveHospitalAsync(request);

                if (isSuccess)
                    return ApiResponse<string>.Success("Hospital, contact, and pharmacy created successfully!!");

                else
                    return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
            }
            catch (Exception e)
            {
                return ApiResponse<string>.InternalServerError(e.Message);
            }
        }

        public async Task<ApiResponse<string>> UpdateHospitalAsync(HospitalRequest request, long id)
        {
            try
            {
                var isSuccess = await _hospitalRepository.UpdateHospitalAsync(request, id);
                if (isSuccess)
                {
                    return ApiResponse<string>.Success(Constants.SUCCESS);
                }
                else
                {
                    return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
                }
            }
            catch (Exception e)
            {
                return ApiResponse<string>.BadRequest(e.Message);
            }
        }
    }
}
