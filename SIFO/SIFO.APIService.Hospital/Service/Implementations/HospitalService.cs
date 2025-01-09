using AutoMapper;
using SIFO.APIService.Hospital.Repository.Contracts;
using SIFO.APIService.Hospital.Service.Contracts;
using SIFO.Common.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Hospital.Service.Implementations
{
    public class HospitalService : IHospitalService
    {
        private readonly IHospitalRepository _hospitalRepository;
        private readonly ICommonService _commonService;
        private readonly IMapper _mapper;

        public HospitalService(IHospitalRepository hospitalRepository, IMapper mapper, ICommonService commonService) 
        {
            _hospitalRepository = hospitalRepository;
            _mapper = mapper;
            _commonService = commonService;
        }

        //public async Task<ApiResponse<HospitalResponse>> GetHospitalByIdAsync(long hospitalId)
        //{
        //    if (hospitalId <= 0)
        //        return ApiResponse<HospitalResponse>.BadRequest();

        //    var startDate = await _commonService.GetStartOfWeek(DateTime.UtcNow.Date);
        //    var response = await _hospitalRepository.GetHospitalByIdAsync(hospitalId);

        //    if (response.Pharmacies.Any())
        //    {
        //        foreach (var pharmacy in response.Pharmacies)
        //        {
        //            pharmacy.Calendar = await _hospitalRepository.GetCalendarByIdAsync(pharmacy.Id, startDate, DateTime.UtcNow.AddDays(6));
        //        }
        //    }

        //    if (response != null)
        //        return ApiResponse<HospitalResponse>.Success(Constants.SUCCESS, response);

        //    return ApiResponse<HospitalResponse>.NotFound();
        //}

        //public async Task<ApiResponse<string>> DeleteHospitalAsync(long hospitalId)
        //{
        //    var response = await _hospitalRepository.DeleteHospitalAsync(hospitalId);
        //    if (response == Constants.NOT_FOUND)
        //        return new ApiResponse<string>(StatusCodes.Status404NotFound, Constants.HOSPITAL_NOT_FOUND);

        //    if (response == Constants.DATA_DEPENDENCY_ERROR_MESSAGE)
        //        return new ApiResponse<string>(StatusCodes.Status400BadRequest, Constants.DATA_DEPENDENCY_ERROR_MESSAGE);

        //    return ApiResponse<string>.Success(Constants.SUCCESS, response);
        //}

        //public async Task<ApiResponse<PagedResponse<HospitalResponse>>> GetAllHospitalAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection)
        //{
        //    var isValid = await HelperService.ValidateGet(pageNo, pageSize, filter, sortColumn, sortDirection);

        //    if (isValid.Any())
        //        return ApiResponse<PagedResponse<HospitalResponse>>.BadRequest(isValid[0]);

        //    var response = await _hospitalRepository.GetAllHospitalAsync(pageNo, pageSize, filter, sortColumn, sortDirection);

        //    return ApiResponse<PagedResponse<HospitalResponse>>.Success(Constants.SUCCESS, response);
        //}

        //public async Task<ApiResponse<string>> CreateHospitalAsync(HospitalRequest request)
        //{
        //    try
        //    {
        //        var tokenData = await _commonService.GetDataFromToken();
        //        bool isSuccess = await _hospitalRepository.CreateHospitalAsync(request);

        //        if (isSuccess)
        //            return ApiResponse<string>.Success("Hospital, contact, and pharmacy created successfully!!");
        //        else
        //            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        //    }
        //    catch (Exception e)
        //    {
        //        return ApiResponse<string>.InternalServerError(e.Message);
        //    }
        //}

        //public async Task<ApiResponse<string>> UpdateHospitalAsync(HospitalRequest request, long id)
        //{
        //    try
        //    {
        //        var addressData = await _commonService.GetAddressDetailByIdAsync(request.AddressId);

        //        if (addressData is null)
        //            return ApiResponse<string>.NotFound();

        //        var isSuccess = await _hospitalRepository.UpdateHospitalAsync(request, id);
        //        if (isSuccess)
        //            return ApiResponse<string>.Success(Constants.SUCCESS);
        //        else
        //            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        //    }
        //    catch (Exception e)
        //    {
        //        return ApiResponse<string>.BadRequest(e.Message);
        //    }
        //}

        public async Task<ApiResponse<Dictionary<string, List<CalendarResponse>>>> GetCalendarByIdAsync(long pharmacyId, DateTime startDate, DateTime endDate)
        {
            if (pharmacyId <= 0)
                return ApiResponse<Dictionary<string, List<CalendarResponse>>>.BadRequest(Constants.BAD_REQUEST);

            if (startDate == DateTime.MinValue || endDate == DateTime.MinValue)
                return ApiResponse<Dictionary<string, List<CalendarResponse>>>.BadRequest(Constants.BAD_REQUEST);

            var pharmacyData = await _hospitalRepository.GetPharmacyByIdAsync(pharmacyId);
            if (!pharmacyData)
                return ApiResponse< Dictionary<string, List<CalendarResponse>>>.BadRequest(Constants.BAD_REQUEST);

            var response = await _hospitalRepository.GetCalendarByIdAsync(pharmacyId, startDate, endDate);

            if (response != null)
                return ApiResponse<Dictionary<string, List<CalendarResponse>>>.Success(Constants.SUCCESS, response);

            return ApiResponse<Dictionary<string, List<CalendarResponse>>>.NotFound(Constants.NOT_FOUND);
        }

        public async Task<ApiResponse<string>> CreateCalendarAsync(CalendarRequest request)
        {
            try
            {
                var pharmacyData = await _hospitalRepository.GetPharmacyByIdAsync(request.PharmacyId); 
                if(!pharmacyData) 
                    return ApiResponse<string>.BadRequest(Constants.BAD_REQUEST);

                var tokenData = await _commonService.GetDataFromToken();
                var mappedResult = _mapper.Map<Calendar>(request);

                mappedResult.CreatedBy = Convert.ToInt64(tokenData.UserId);
                string calendarData = await _hospitalRepository.CreateCalendarAsync(mappedResult);

                if (calendarData == Constants.SUCCESS)
                    return ApiResponse<string>.Created(Constants.SUCCESS);
                return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);

            }
            catch (Exception ex)
            {
                return ApiResponse<string>.InternalServerError(ex.Message);
            }
        }

        public async Task<ApiResponse<string>> UpdateCalendarAsync(CalendarRequest request)
        {
            try
            {
                var tokenData = await _commonService.GetDataFromToken();
                var pharmacyData = await _hospitalRepository.GetPharmacyByIdAsync(request.PharmacyId);
                if (!pharmacyData)
                    return ApiResponse<string>.BadRequest(Constants.BAD_REQUEST);

                bool isCalendarExists = await _hospitalRepository.CalendarExistsAsync(request.id.Value); 
                if (!isCalendarExists) 
                    return ApiResponse<string>.NotFound(Constants.NOT_FOUND);

                var mappedResult = _mapper.Map<Calendar>(request);
                mappedResult.UpdatedBy = Convert.ToInt64(tokenData.UserId);
                mappedResult.UpdatedDate = DateTime.UtcNow;

                var result = await _hospitalRepository.UpdateCalendarAsync(mappedResult);

                if (result == Constants.SUCCESS)
                    return ApiResponse<string>.Success(Constants.SUCCESS); 
                return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.InternalServerError(ex.Message);
            }
        }
    }
}
