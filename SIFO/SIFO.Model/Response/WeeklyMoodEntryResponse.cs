namespace SIFO.Model.Response
{
    public class WeeklyMoodEntryResponse
    {
        public long Id { get; set; }
        public string ColorCode { get; set; }
        public string ImagePath { get; set; }
        public long PatientId { get; set; }
        public DateTime? WeekStartDate { get; set; }
        public DateTime? WeekEndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
