using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;
using Twilio.Http;

namespace SIFO.Common.Contracts
{
    public interface ICommonService
    {
        public Task<string> GenerateOTP(long length);
        public Task<string> GenerateRandomPassword(long length);
        public Task<string> GenerateAccessToken(Users user, bool rememberMe);
        public Task<string> GenerateRefreshToken(long userId);
        public Task<TokenResponse?> GetDataFromToken();
        public Task<string> EncryptPassword(string password);
        public Task<string> DecryptPassword(string encryptedText);
        public Task<bool> SendMail(List<string> to, List<string>? cc, string subject, string body);
        public Task<bool> SendSms(List<string> phoneNumbers, string body);
        public void TrimStrings(ref string? filter, ref string? sortColumn, ref string? sortDirection);
        public Task<string> SaveFileAsync(string base64File, string? fileType, string destinationFolder);
        public Task<AuthenticationType> GetAuthenticationTypeByIdAsync(long Id);
        public Task<OtpRequest> CreateOtpRequestAsync(long userId, string authenticationFor, long authenticationType);
        public Task<string> SendOtpRequestAsync(long userId, string authenticationFor, long authenticationType);

        public Task<PagedResponse<AddressDetailResponse>> GetAllAddressDetailAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll = false);
        public Task<AddressDetailResponse> GetAddressDetailByIdAsync(long id);
        public Task<string> DeleteAddressDetailAsync(long id);
        public Task<AddressDetail> CreateAddressDetailAsync(AddressDetail entity);
        public Task<long> AddressDetailExistsAsync(string? address, long? cityId, long? region, long? countryId, long? zipcode);
        public Task<AddressDetail> UpdateAddressDetailAsync(AddressDetail entity);
        public Task<bool> AddressDetailExistsByIdAsync(long? id);
        public Task<string> GetIpAddress();
        public Task<DateTime> GetStartOfWeek(DateTime date);
        public Task<string> HashPassword(string password);
    }
}
