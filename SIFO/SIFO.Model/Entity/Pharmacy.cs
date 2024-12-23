using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SIFO.Model.Entity
{
    [Table("pharmacies")]
    public class Pharmacy
    {
        [Column("Id")]
        [Key]
        public long Id { get; set; }

        [Column("PharmacyName")]
        public string PharmacyName { get; set; }

        [Column("PharmacyTypeId")]
        public long PharmacyTypeId { get; set; }

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

        [Column("validFrom")]
        public DateTime? ValidFrom { get; set; }

        [Column("validTo")]
        public DateTime? ValidTo { get; set; }

        [Column("ASL")]
        public string? ASL { get; set; }

        [Column("MinisterialID")]
        public string? MinisterialID { get; set; }

        [Column("CAP")]
        public string? CAP { get; set; }

        [Column("Province")]
        public string? Province { get; set; }

        [Column("alertSent")]
        public int AlertSent { get; set; }

        [Column("AddressId")]
        public long AddressId { get; set; }
    }
}
