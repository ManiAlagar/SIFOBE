namespace SIFO.Model.Request
{
    public class Login2FARequest
    { 
        public long AuthenticationType { get; set; }
        public string AuthenticationFor { get; set; }
        public string OtpCode { get; set; }
        public long UserId { get; set; }
    }
}
