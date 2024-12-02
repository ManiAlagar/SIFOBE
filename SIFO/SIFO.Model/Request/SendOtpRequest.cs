namespace SIFO.Model.Request
{
    public class SendOtpRequest
    {
        public long? UserId { get; set; }
        public string AuthenticationFor { get; set; }
        public long AuthenticationType { get; set; } 
        public string? Email {  get; set; }
    }
}
