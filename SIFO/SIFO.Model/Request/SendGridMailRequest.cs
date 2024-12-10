namespace SIFO.Model.Request
{
    public class SendGridMailRequest
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Name { get; set; }
    }
}
