namespace SIFO.Model.Request
{
    public class VerifyOtpRequest
    { 
        public long UserId { get; set; }
        public string OtpCode { get; set; } 
        public string AuthenticationFor { get; set;} 
        public string? Email { get; set;} 
        public long AuthenticationType { get; set; }
    }
}
