namespace SIFO.Model.Request
{
    public class CalendarRequest
    {  
        public long? id { get; set; }
        public long PharmacyId { get; set; }
        public TimeSpan? OpeningTime { get; set; }
        public TimeSpan? ClosingTime { get; set; }
        public DateTime CalendarDate { get; set; } 
        public bool IsHoliday {  get; set; }
    }
}
