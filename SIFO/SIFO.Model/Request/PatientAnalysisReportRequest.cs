using Microsoft.AspNetCore.Http;

namespace SIFO.Model.Request
{
    public class PatientAnalysisReportRequest
    {
        public long PatientId { get; set; }
        public string File { get; set; }
        public string Attachment_Type { get; set; }
        public string? Description { get; set; }
    }
}
