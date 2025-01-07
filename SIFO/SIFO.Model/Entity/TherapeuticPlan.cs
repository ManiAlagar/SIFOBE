using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    [Table("therapeutic_plan")]
    public class TherapeuticPlan
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; } 

        [Column("Date_From")]
        public DateTime DateFrom { get; set; }  

        [Column("Date_To")]
        public DateTime DateTo { get; set; } 

        [Column("Fk_HospitalFacilityId")]
        public long HospitalFacilityId { get; set; } 

        [Column("CreatedBy")]
        public long CreatedBy { get; set; } 

        [Column("UpdatedBy")]
        public long? UpdatedBy { get; set; }  

        [Column("CreatedDate")]
        [Required]
        public DateTime CreatedDate { get; set; } 

        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }  
    }
}
