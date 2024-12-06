namespace SIFO.Model.Response
{
    public class CalendarResponse
    {
        public TimeSpan? OpeningTime { get; set; }
        public TimeSpan? ClosingTime { get; set; }
        public DateTime? CalendarDate { get; set; }
        public bool IsHoliday { get; set; }
        public long? PharmacyId { get; set; }
    }
}
