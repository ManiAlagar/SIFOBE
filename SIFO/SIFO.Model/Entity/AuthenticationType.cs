using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SIFO.Model.Entity
{
    [Table("authenticationtypes")]
    public class AuthenticationType
    {
        [Key]
        public long Id { get; set; }

        [Column("authenticationtype")]
        public string AuthType { get; set; }

        [Column("description")]
        public string Description { get; set; }
    }
}
