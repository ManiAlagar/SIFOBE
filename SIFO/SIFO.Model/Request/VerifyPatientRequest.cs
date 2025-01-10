namespace SIFO.Model.Request
{
    public class VerifyPatientRequest
    { 
        public string PhoneNumber { get; set; }
        public string OtpCode { get; set;} 
        public long AuthenticationType { get; set;} 
        public string AuthenticationFor { get; set;}
    }
}
