using SIFO.Model.Constant;
using SIFO.Model.Request;
using SIFO.Model.Response;
using SIFO.Common.Contracts;
using SIFO.Utility.Implementations;
using SIFO.APIService.Hospital.Service.Contracts;
using SIFO.APIService.Hospital.Repository.Contracts;

namespace SIFO.APIService.Hospital.Service.Implementations
{
    public class PharmacyService : IPharmacyService
    {
        private readonly IPharmacyRepository _pharmacyRepository;
        private readonly IHospitalRepository _hospitalRepository;
        private readonly ICommonService _commonService;

        public PharmacyService(IPharmacyRepository pharmacyRepository, ICommonService commonService, IHospitalRepository hospitalRepository)
        {
            _pharmacyRepository = pharmacyRepository;
            _commonService = commonService;
            _hospitalRepository = hospitalRepository;
        }

        public async Task<ApiResponse<PharmacyDetailResponse>> GetPharmacyByIdAsync(long pharmacyId)
        {
            if (pharmacyId <= 0)
                return ApiResponse<PharmacyDetailResponse>.BadRequest(Constants.BAD_REQUEST);

            var startDate = await _commonService.GetStartOfWeek(DateTime.UtcNow.Date);
            var response = await _pharmacyRepository.GetPharmacyByIdAsync(pharmacyId);
            response.Calendar = await _hospitalRepository.GetCalendarByIdAsync(pharmacyId, startDate, startDate.AddDays(6));
            if (response != null)
                return ApiResponse<PharmacyDetailResponse>.Success(Constants.SUCCESS, response);

            return ApiResponse<PharmacyDetailResponse>.NotFound(Constants.NOT_FOUND);
        }

        public async Task<ApiResponse<string>> CreatePharmacyAsync(PharmacyRequest request)
        {
            var isMobileExists = await _pharmacyRepository.IsPhoneNumberExists(request.PhoneNumber);
            if (isMobileExists)
                return ApiResponse<string>.Conflict(Constants.PHONE_ALREADY_EXISTS);

            var pharmacyType = await _pharmacyRepository.GetPharmacyTypeByIdAsync(request.PharmacyTypeId); 

            if (pharmacyType is null) 
                return ApiResponse<string>.NotFound(Constants.PHARMACY_ID_NOT_EXISTS);

            bool isMinisterialExists = default;
            if (pharmacyType.Name.ToLower() == "retail")
            {
                if (string.IsNullOrEmpty(request.MinisterialId))
                    return ApiResponse<string>.BadRequest(Constants.BAD_REQUEST);  
                isMinisterialExists = await _pharmacyRepository.IsRetailExists(request.MinisterialId);
            } 
            if(isMinisterialExists)
                return ApiResponse<string>.Conflict(Constants.MINISTERIAL_ID_EXISTS);

            bool isSuccess = await _pharmacyRepository.CreatePharmacyAsync(request);
            if (isSuccess)
                return ApiResponse<string>.Success(Constants.PHARMACY_AND_CONTACT_CREATED_SUCCESSFULLY);
            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<string>> UpdatePharmacyAsync(PharmacyRequest request, long pharmacyId)
        {
            if (pharmacyId <= 0)
                return ApiResponse<string>.BadRequest();

            var isMobileExists = await _pharmacyRepository.IsPhoneNumberExists(request.PhoneNumber);
            if (isMobileExists)
                return ApiResponse<string>.Conflict(Constants.PHONE_ALREADY_EXISTS);

            bool isMinisterialExists = default;
            var addressData = await _commonService.GetAddressDetailByIdAsync(request.AddressId);
            if (addressData is not null)
                request.AddressId = addressData.Id;

            var pharmacyType = await _pharmacyRepository.GetPharmacyTypeByIdAsync(request.PharmacyTypeId);
            if (pharmacyType is null)
                return ApiResponse<string>.NotFound(Constants.PHARMACY_ID_NOT_EXISTS);

            if (pharmacyType.Name == "retail")
            {
                if (string.IsNullOrEmpty(request.MinisterialId))
                    return ApiResponse<string>.BadRequest();
                isMinisterialExists = await _pharmacyRepository.IsRetailExists(request.MinisterialId);
            }
            if (isMinisterialExists)
                return ApiResponse<string>.Conflict(Constants.MINISTERIAL_ID_EXISTS);

            var isSuccess = await _pharmacyRepository.UpdatePharmacyAsync(request, pharmacyId);
            if (isSuccess)
                return ApiResponse<string>.Success(Constants.SUCCESS);

            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<string>> DeletePharmacyAsync(long pharmacyId)
        {
            if (pharmacyId <= 0)
                return ApiResponse<string>.BadRequest(Constants.BAD_REQUEST);

            var response = await _pharmacyRepository.DeletePharmacyAsync(pharmacyId);
            if (response == Constants.NOT_FOUND)
                return ApiResponse<string>.NotFound(Constants.PHARMACY_NOT_FOUND);

            return ApiResponse<string>.Success(Constants.SUCCESS, response);
        }

        public async Task<ApiResponse<PagedResponse<PharmaciesResponse>>> GetPharmacyAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, string pharmacyType, bool isCurrentUser)
        {
            var tokenData = await _commonService.GetDataFromToken();
            if (pharmacyType is not null)
            {
                if (!Enum.IsDefined(typeof(Constants.PharmacyTypes), pharmacyType.ToLower()))
                    return ApiResponse<PagedResponse<PharmaciesResponse>>.BadRequest(Constants.BAD_REQUEST);
            }

            var isValid = await HelperService.ValidateGet(pageNo, pageSize, filter, sortColumn, sortDirection);
            if (isValid.Any())
                return ApiResponse<PagedResponse<PharmaciesResponse>>.BadRequest(isValid[0]);

            if (!isCurrentUser)
            {
                if (tokenData.Role.ToLower() == Constants.ROLE_QC_ADMINISTRATOR.ToLower() || tokenData.Role.ToLower() == Constants.ROLE_SUPER_ADMIN.ToLower())
                {
                    var result = await _pharmacyRepository.GetPharmacyAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll, pharmacyType, isCurrentUser, tokenData.UserId);
                    return ApiResponse<PagedResponse<PharmaciesResponse>>.Success(Constants.SUCCESS, result);
                }
                else
                    return ApiResponse<PagedResponse<PharmaciesResponse>>.Forbidden(Constants.ACCESS_DENIED);
            }

            var response = await _pharmacyRepository.GetPharmacyAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll, pharmacyType, isCurrentUser, tokenData.UserId);
            return ApiResponse<PagedResponse<PharmaciesResponse>>.Success(Constants.SUCCESS, response);
        }
    }
}
