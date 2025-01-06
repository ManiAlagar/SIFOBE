namespace SIFO.Model.Request
{
    public class VerifyPatientRequest
    { 
        public string PatientCode { get; set; }
        public string OtpCode { get; set;} 
        public long AuthenticationType { get; set;} 
        public string AuthenticationFor { get; set;}
    }
}
