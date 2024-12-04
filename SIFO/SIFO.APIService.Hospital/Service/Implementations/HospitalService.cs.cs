using SIFO.APIService.Hospital.Repository.Contracts;
using SIFO.APIService.Hospital.Service.Contracts;
using SIFO.Common.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Hospital.Service.Implementations
{
    public class HospitalService : IHospitalService
    {
        private readonly IHospitalRepository _hospitalRepository;
        private readonly ICommonService _commonService;
        public HospitalService(IHospitalRepository hospitalRepository, ICommonService commonService)
        {
            _hospitalRepository = hospitalRepository;
            _commonService = commonService;
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

        public async Task<ApiResponse<string>> UpdateHospitalAsync(HospitalRequest request,long id)
        {
            try
            {
              var isSuccess= await _hospitalRepository.UpdateHospitalAsync(request,id);
                if (isSuccess)
                {
                    return ApiResponse<string>.Success(Constants.SUCCESS);
                }
                else
                {
                    return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
                }
            }
            catch( Exception e) 
            {
                return ApiResponse<string>.BadRequest(e.Message);
            }
        }

    }
}
