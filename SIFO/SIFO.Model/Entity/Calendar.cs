using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SIFO.Model.Entity
{
    [Table("calendar")] 
    public class Calendar
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("pharmacyId")] 
        public long? PharmacyId { get; set; }

        [Column("openingTime")]
        public TimeSpan OpeningTime { get; set; }

        [Column("closingTime")] 
        public TimeSpan ClosingTime { get; set; }

        [Column("calendarDate")]
        public DateTime? CalendarDate { get; set; } 

        [Column("holiday")] 
        public bool IsHoliday { get; set; }

        [Column("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [Column("createdBy")]
        public long CreatedBy { get; set; }

        [Column("updatedDate")]
        public DateTime? UpdatedDate { get; set; }

        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
    }
}
