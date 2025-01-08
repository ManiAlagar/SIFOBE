namespace SIFO.Model.Request
{
    public class VerifyTwilio2FARequest
    {
        public string AuthyId { get; set; }
        public string Code { get; set; }
    }
}
