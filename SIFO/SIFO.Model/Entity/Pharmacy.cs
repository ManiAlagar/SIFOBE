using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    [Table("pharmacies")]
    public class Pharmacy
    {
        [Column("Id")]
        [Key]
        public long Id { get; set; }

        [Column("HospitalStructureId")]
        public long HospitalId { get; set; }

        [Column("PharmacyName")]
        public string PharmacyName { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("CreatedBy")]
        public long? CreatedBy { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Column("UpdatedBy")]
        public long? UpdatedBy { get; set; }

        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }

        [Column("PharmacyTypeId")]
        public long PharmacyTypeId { get; set; }

        [NotMapped]
        public string? PharmacyType { get; set; }
    }
}
