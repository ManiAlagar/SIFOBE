using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    [Table("therapeutic_plan_details")]
    public class TherapeuticPlanDetails
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; } 

        [Column("Fk_DrugId")]
        public long? DrugId { get; set; }

        [Column("Fk_TherapeuticPlanId")]
        public long? TherapeuticPlanId { get; set; }

        [Column("Fk_PharmacyId")]
        public long? PharmacyId { get; set; }  

        [Column("UMR_Period")]
        public long? UMRPeriod { get; set; }  

        [Column("Period")]
        public string? Period { get; set; }

        [Column("Dosage_Times")]
        public int? DosageTimes { get; set; }  

        [Column("Frequency_intake")]
        public string? FrequencyIntake { get; set; } 

        [Column("Type_intake")]
        public string? TypeIntake { get; set; } 

        [Column("UMR_intake")]
        public long? UMRIntake { get; set; }  

        [Column("Drug_Dosage")]
        public string DrugDosage { get; set; }  

        [Column("From")]
        public DateTime? From { get; set; } 

        [Column("Take_On")]
        public DateTime? TakeOn { get; set; }  
    }
}
