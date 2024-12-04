using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Master.Service.Contracts
{
    public interface IAddressService
    {
        public Task<ApiResponse<PagedResponse<AddressDetailResponse>>> GetAllAddressDetailAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll);
        public Task<ApiResponse<AddressDetailResponse>> GetAddressDetailByIdAsync(long addressId);
        public Task<ApiResponse<string>> DeleteAddressDetailAsync(long id);
        public Task<ApiResponse<AddressDetail>> CreateAddressDetailAsync(AddressDetailRequest entity);
        public Task<ApiResponse<AddressDetail>> UpdateAddressDetailAsync(AddressDetailRequest entity);

    }
}
