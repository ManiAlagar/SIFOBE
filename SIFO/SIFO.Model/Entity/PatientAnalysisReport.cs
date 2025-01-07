using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    [Table("patient_analysis_report")]
    public class PatientAnalysisReport
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Required]
        [Column("Fk_PatientId")]
        public long PatientId { get; set; }

        [Required]
        [Column("FilePath")]
        public string FilePath { get; set; }

        [Required]
        [Column("Attachment_Type")]
        public string Attachment_Type { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        [Required]
        [Column("createdBy")]
        public long CreatedBy { get; set; }

        [Column("createdDate")]
        public DateTime? CreatedDate { get; set; }
    }
}
