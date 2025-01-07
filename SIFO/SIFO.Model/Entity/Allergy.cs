using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SIFO.Model.Entity
{
    [Table("allergy")]
    public class Allergy
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Column("Fk_PatientId")]
        public long PatientId { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }

        [Column("CreatedBy")]
        public long CreatedBy { get; set; }

        [Column("UpdatedBy")]
        public long? UpdatedBy { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }
    }
}
