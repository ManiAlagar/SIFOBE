using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SIFO.Model.Entity
{
    [Table("usersessionmanagement")]
    public class UserSessionManagement
    {
        [Key]
        [Column("Id")]
        public long? Id { get; set; }

        [Column("userId")]
        public long UserId { get; set; }

        [Column("IPAccess")]
        public string IPAccess { get; set; }

        [Column("tokenSession")]
        public string TokenSession { get; set; }

        [Column("dtLogin")]
        public DateTime? DtLogin { get; set; }

        [Column("dtLogout")]
        public DateTime? DtLogout { get; set; }

        [Column("dtCreation")]
        public DateTime DtCreation { get; set; }

    }
}
