namespace SIFO.Model.Request
{
    public class ChangePasswordRequest
    {
        public long UserId { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string OtpCode { get; set; }
        public long AuthenticationType { get; set; }
        public string AuthenticationFor { get; set; }
    }
}
