namespace SIFO.Model.Response
{
    public class TotpResponse
    {
        public string Message { get; set; }
        public string Sid { get; set; }
        public BindingsResponse bindingsResponse { get; set; }
    }
}
