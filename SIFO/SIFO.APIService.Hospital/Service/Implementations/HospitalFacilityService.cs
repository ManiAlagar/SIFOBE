using SIFO.Model.Request;
using SIFO.Model.Constant;
using SIFO.Model.Response;
using SIFO.Common.Contracts;
using SIFO.Utility.Implementations;
using SIFO.APIService.Hospital.Service.Contracts;
using SIFO.APIService.Hospital.Repository.Contracts;

namespace SIFO.APIService.Hospital.Service.Implementations
{
    public class HospitalFacilityService : IHospitalFacilityService
    {
        private readonly IHospitalFacilityRepository _hospitalFacilityRepository;
        private readonly ICommonService _commonService;
        private readonly IPharmacyRepository _pharmacyRepository;

        public HospitalFacilityService(IHospitalFacilityRepository hospitalFacilityRepository, ICommonService commonService, IPharmacyRepository pharmacyRepository)
        {
            _hospitalFacilityRepository = hospitalFacilityRepository;
            _commonService = commonService;
            _pharmacyRepository = pharmacyRepository;
        }

        public async Task<ApiResponse<PagedResponse<HospitalFacilityResponse>>> GetAllHospitalFacilityAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll)
        {
            var isValid = await HelperService.ValidateGet(pageNo, pageSize, filter, sortColumn, sortDirection);

            if (isValid.Any())
                return ApiResponse<PagedResponse<HospitalFacilityResponse>>.BadRequest(isValid[0]);

            var response = await _hospitalFacilityRepository.GetAllHospitalFacilityAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll);

            return ApiResponse<PagedResponse<HospitalFacilityResponse>>.Success(Constants.SUCCESS, response);
        }

        public async Task<ApiResponse<HospitalFacilityDetailResponse>> GetHospitalFacilityByIdAsync(long hospitalFacilityId)
        {
            if (hospitalFacilityId <= 0)
                return ApiResponse<HospitalFacilityDetailResponse>.BadRequest();

            var response = await _hospitalFacilityRepository.GetHospitalFacilityByIdAsync(hospitalFacilityId);

            if (response != null)
                return ApiResponse<HospitalFacilityDetailResponse>.Success(Constants.SUCCESS, response);

            return ApiResponse<HospitalFacilityDetailResponse>.NotFound();
        }

        public async Task<ApiResponse<string>> CreateHospitalFacilityAsync(HospitalFacilityRequest request)
        {
            var tokenData = await _commonService.GetDataFromToken();

            var pharmacyResponse = await _hospitalFacilityRepository.GetPharmaciesByIdsAsync(request.PharmacyIds);
            if (pharmacyResponse.Count != request.PharmacyIds.Count)
                return ApiResponse<string>.NotFound("pharmacy id does not exists.");

            var retailPharmacyId = await _pharmacyRepository.GetRetailPharmacyAsync();
            var result = pharmacyResponse.Where(a => a.PharmacyTypeId == retailPharmacyId).ToList();
            if (result.Count > 0)
                return ApiResponse<string>.BadRequest("only hospital pharmacies can be created. Please provide a valid pharmacy Ids.");

            bool isSuccess = await _hospitalFacilityRepository.CreateHospitalFacilityAsync(request);

            if (isSuccess)
                return ApiResponse<string>.Success("hospital facility, contact, and pharmacy created successfully!!");
            
            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<string>> UpdateHospitalFacilityAsync(HospitalFacilityRequest request, long hospitalFacilityId)
        {
            if (hospitalFacilityId <= 0)
                return ApiResponse<string>.BadRequest();

            var addressData = await _commonService.GetAddressDetailByIdAsync(request.AddressId);

            if (addressData is not null)
                request.AddressId = addressData.Id;

            var pharmacyResponse = await _hospitalFacilityRepository.GetPharmaciesByIdsAsync(request.PharmacyIds);
            if (pharmacyResponse.Count != request.PharmacyIds.Count)
                return ApiResponse<string>.NotFound("pharmacy id does not exists.");

            var retailPharmacyId = await _pharmacyRepository.GetRetailPharmacyAsync();
            var result = pharmacyResponse.Where(a => a.PharmacyTypeId == retailPharmacyId).ToList();
            if (result.Count > 0)
                return ApiResponse<string>.BadRequest("only hospital pharmacies can be created. Please provide a valid pharmacy Ids.");

            var isSuccess = await _hospitalFacilityRepository.UpdateHospitalFacilityAsync(request, hospitalFacilityId);
            if (isSuccess)
                return ApiResponse<string>.Success(Constants.SUCCESS);
            
            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<string>> DeleteHospitalFacilityAsync(long hospitalFacilityId)
        {
            if (hospitalFacilityId <= 0)
                return ApiResponse<string>.BadRequest();

            var result = await _hospitalFacilityRepository.GetHospitalFacilityByIdAsync(hospitalFacilityId);
            if (result is null)
                return ApiResponse<string>.NotFound(Constants.HOSPITAL_FACILITY_NOT_FOUND);

            var response = await _hospitalFacilityRepository.DeleteHospitalFacilityAsync(hospitalFacilityId);
            if (response == Constants.SUCCESS)
                return ApiResponse<string>.Success(Constants.SUCCESS, response);

            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }
    }
}
