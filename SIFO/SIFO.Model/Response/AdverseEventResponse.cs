namespace SIFO.Model.Response
{
    public class AdverseEventResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public string Intensity { get; set; }
        public bool IsActive { get; set; }
    }
}
