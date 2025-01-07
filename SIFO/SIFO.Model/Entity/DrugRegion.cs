using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIFO.Model.Entity
{
    [Table("DrugsRegions")]
    public class DrugRegion
    {
        [Key]
        [Column("DrugsRegionsId")]
        public long DrugsRegionsId { get; set; }

        [Column("RegionId")]
        public long RegionId { get; set; }
        [Column("DrugType")]
        public string DrugType { get; set; }

        [Column("DrugId")]
        public long DrugId { get; set; }
        [Column("CreatedBy")]
        public long CreatedBy { get; set; }
        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
