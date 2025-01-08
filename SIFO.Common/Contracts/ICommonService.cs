using SIFO.Model.Response;

namespace SIFO.Common.Contracts
{
    public interface ICommonService
    {
        public Task<string> GenerateOTP(long length);
        public Task<string> GenerateRandomPassword(long length);
        public Task<TokenResponse?> GetDataFromToken();
        public Task<string> EncryptPassword(string password);
        public Task<string> DecryptPassword(string encryptedText);
        public Task<bool> SendMail(List<string> to, List<string>? cc, string subject, string body);
        public void TrimStrings(ref string? filter, ref string? sortColumn, ref string? sortDirection);
    }
}
