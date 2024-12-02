namespace SIFO.Model.Request
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string OtpCode { get; set; }
        public long UserId { get; set; }
        public string AuthenticationFor { get; set; }
        public long AuthenticationType { get; set; }
    }
}
