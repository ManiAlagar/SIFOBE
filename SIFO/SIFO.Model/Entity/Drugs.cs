using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIFO.Model.Entity
{
    [Table("Drugs")]
    public class Drug
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Column("DD")]
        public bool DD { get; set; } = false;

        [Column("DPC")]
        public bool DPC { get; set; } = false;

        [Column("InPharmacy")]
        public bool InPharmacy { get; set; } = false;

        [Required]
        [Column("AIC")]
        public string AIC { get; set; }

        [Required]
        [Column("ExtendedDescription")]
        public string ExtendedDescription { get; set; }

        [Required]
        [Column("CompanyName")]
        public string CompanyName { get; set; }

        [Required]
        [Column("Price", TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        [Column("ProductType")]
        public string ProductType { get; set; }

        [Column("Class")]
        public string Class { get; set; }

        [Column("PharmaceuticalForm")]
        public string PharmaceuticalForm { get; set; }

        [Column("UMR")]
        public long? UMR { get; set; }

        [Column("PrescriptionType")]
        public string PrescriptionType { get; set; }

        [Column("ProductImage")]
        public string ProductImage { get; set; }

        [Column("TherapeuticIndications")]
        public string TherapeuticIndications { get; set; }

        [Column("Temperature")]
        public string Temperature { get; set; }

        [Column("NumberGGAlert")]
        public long? NumberGGAlert { get; set; }

        [Column("AlertHours")]
        public int? AlertHours { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Required]
        [Column("CreatedBy")]
        public long CreatedBy { get; set; }

        [Column("UpdatedBy")]
        public long? UpdatedBy { get; set; }
    }

}

