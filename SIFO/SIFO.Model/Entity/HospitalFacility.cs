using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SIFO.Model.Entity
{
    [Table("hospitalfacilities")]
    public class HospitalFacility
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("AddressId")]
        public long AddressId { get; set; }

        [Column("ASL")]
        public string? ASL { get; set; }

        [Column("Province")]
        public string Province { get; set; }

        [Column("PhoneNumber")]
        public string PhoneNumber { get; set; }

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

        [Column("CAP")]
        public string? CAP { get; set; }
    }
}
