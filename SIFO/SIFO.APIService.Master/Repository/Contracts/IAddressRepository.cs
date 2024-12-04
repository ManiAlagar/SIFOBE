using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Master.Repository.Contracts
{
    public interface IAddressRepository
    {
        public Task<PagedResponse<AddressDetailResponse>> GetAllAddressDetailAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll = false);
        public Task<AddressDetailResponse> GetAddressDetailByIdAsync(long id);
        public Task<string> DeleteAddressDetailAsync(long id);
        public Task<AddressDetail> CreateAddressDetailAsync(AddressDetail entity);
        public Task<bool> AddressDetailExistsAsync(AddressDetailRequest entity);
        public Task<AddressDetail> UpdateAddressDetailAsync(AddressDetail entity);
        public Task<bool> AddressDetailExistsByIdAsync(long? id);
    }
}
