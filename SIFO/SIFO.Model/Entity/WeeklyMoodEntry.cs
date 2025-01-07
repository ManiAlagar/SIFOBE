using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    [Table("weekly_mood_entry")]
    public class WeeklyMoodEntry
    {
        [Column("Id")]
        public long Id { get; set; }

        [Column("color_Code")]
        public string ColorCode { get; set; }

        [Column("Image_path")]
        public string ImagePath { get; set; }

        [Column("Fk_PatientId")]
        public long PatientId { get; set; }

        [Column("Week_Start_Date")]
        public DateTime? WeekStartDate { get; set; }

        [Column("Week_End_Date")]
        public DateTime? WeekEndDate { get; set; }

        [Column("CreatedBy")]
        public long CreatedBy { get; set; }

        [Column("UpdatedBy")]
        public long? UpdatedBy { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }
    }
}
