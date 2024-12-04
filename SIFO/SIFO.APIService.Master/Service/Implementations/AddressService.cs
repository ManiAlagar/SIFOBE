using AutoMapper;
using SIFO.Model.Constant;
using SIFO.Model.Response;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Utility.Implementations;
using SIFO.APIService.Master.Service.Contracts;
using SIFO.APIService.Master.Repository.Contracts;
using SIFO.APIService.Master.Repository.Implementations;


namespace SIFO.APIService.Master.Service.Implementations
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        public AddressService(IAddressRepository addressRepository, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PagedResponse<AddressDetailResponse>>> GetAllAddressDetailAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll)
        {
            var isValid = await HelperService.ValidateGet(pageNo, pageSize, filter, sortColumn, sortDirection);

            if (isValid.Any())
                return ApiResponse<PagedResponse<AddressDetailResponse>>.BadRequest(isValid[0]);

            var response = await _addressRepository.GetAllAddressDetailAsync(pageNo, pageSize, filter, sortColumn, sortDirection, isAll);

            return ApiResponse<PagedResponse<AddressDetailResponse>>.Success(Constants.SUCCESS, response);
        }

        public async Task<ApiResponse<AddressDetailResponse>> GetAddressDetailByIdAsync(long id)
        {
            if (id <= 0)
                return ApiResponse<AddressDetailResponse>.BadRequest();

            var response = await _addressRepository.GetAddressDetailByIdAsync(id);

            if (response != null)
                return ApiResponse<AddressDetailResponse>.Success(Constants.SUCCESS, response);

            return ApiResponse<AddressDetailResponse>.NotFound();
        }

        public async Task<ApiResponse<string>> DeleteAddressDetailAsync(long id)
        {
            var response = await _addressRepository.DeleteAddressDetailAsync(id);
            if (response == Constants.NOT_FOUND)
                return new ApiResponse<string>(StatusCodes.Status404NotFound, Constants.ADDRESSDETAIL_NOT_FOUND);
            if (response == Constants.DATADEPENDENCYERRORMESSAGE)
                return new ApiResponse<string>(StatusCodes.Status400BadRequest, Constants.DATADEPENDENCYERRORMESSAGE);
            return ApiResponse<string>.Success(Constants.SUCCESS, response);
        }

        public async Task<ApiResponse<AddressDetail>> CreateAddressDetailAsync(AddressDetailRequest entity)
        {
            bool isNameExists = await _addressRepository.AddressDetailExistsAsync(entity);

            if (isNameExists)
                return new ApiResponse<AddressDetail>(StatusCodes.Status409Conflict, Constants.ADDRESSDETAIL_ALREADY_EXISTS);

            var mappedResult = _mapper.Map<AddressDetail>(entity);

            var response = await _addressRepository.CreateAddressDetailAsync(mappedResult);

            if (response.Id > 0)
                return ApiResponse<AddressDetail>.Success(Constants.SUCCESS, response);

            return new ApiResponse<AddressDetail>(StatusCodes.Status500InternalServerError);
        }

        public async Task<ApiResponse<AddressDetail>> UpdateAddressDetailAsync(AddressDetailRequest entity)
        {
            bool isExists = await _addressRepository.AddressDetailExistsByIdAsync(entity.Id);

            if (!isExists)
                return new ApiResponse<AddressDetail>(StatusCodes.Status404NotFound, Constants.ADDRESSDETAIL_NOT_FOUND);
            var mappedResult = _mapper.Map<AddressDetail>(entity);

            var response = await _addressRepository.UpdateAddressDetailAsync(mappedResult);

            if (response != null)
            {
                return ApiResponse<AddressDetail>.Success(Constants.SUCCESS, response);
            }
            return new ApiResponse<AddressDetail>(StatusCodes.Status500InternalServerError);
        }
    }
}
