using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SIFO.Model.Entity
{
    [Table("adverse_event")]
    public class AdverseEvent
    {
        [Key]  
        [Column("Id")]  
        public long Id { get; set; }

        [Column("Name")]  
        public string Name { get; set; }

        [Column("Fk_PatientId")]
        public long PatientId { get; set; }

        [Column("Description")]  
        public string? Description { get; set; }  

        [Column("Date")]  
        public DateTime? Date { get; set; }  
        
        [Column("Intensity")]  
        public string Intensity { get; set; } 
      
        [Column("CreatedBy")]  
        public long CreatedBy { get; set; }  

        [Column("UpdatedBy")]  
        public long? UpdatedBy { get; set; }  

        [Column("CreatedDate")]  
        public DateTime CreatedDate { get; set; } = DateTime.Now; 

        [Column("UpdatedDate")]  
        public DateTime? UpdatedDate { get; set; } 

        [Column("IsActive")] 
        public bool IsActive { get; set; } 
    }
}
