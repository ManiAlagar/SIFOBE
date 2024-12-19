using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    [Table("facilitiespharmaciesmapping")]
    public class FacilityPharmacyMapping
    {
        [Column("Id")]
        [Key]
        public long Id { get; set; }

        [Column("FacilityId")]
        public long FacilityId { get; set; }

        [Column("PharmacyId")]
        public long PharmacyId { get; set; } 
    }
}
