using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
   
        [Table("m2m_user_hospital")]
        public class UserHospitalMapping
        {
            [Column("Id")]
            [Key]
            public long Id { get; set; }

            [Column("fk_user")]
            public long? UserId { get; set; }

            [Column("fk_hospital")]
            public long? HospitalId { get; set; }
        }
    }

