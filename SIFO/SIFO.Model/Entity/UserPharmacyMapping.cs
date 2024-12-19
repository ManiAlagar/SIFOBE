using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIFO.Model.Entity
{
    public class UserPharmacyMapping
    {
        [Column("Id")]
        [Key]
        public long Id { get; set; }

        [Column("fk_user")]
        public long? userId { get; set; }

        [Column("fk_pharmacy")]
        public long? PharmacyId { get; set; }
    }
}
