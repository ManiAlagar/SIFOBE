namespace SIFO.Model.Response
{
    public class PatientAnalysisReportResponse
    {
        public long Id { get; set; }
        public long PatientId { get; set; }
        public string FilePath { get; set; }
        public string Attachment_Type { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
