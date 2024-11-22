namespace SIFO.Model.Request
{
    public class Login2FARequest
    {
        public string  OTPMethod { get; set; } 
        public long UserId { get; set; }
    }
}
