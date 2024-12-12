using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    [Table("roles")]
    public class Role
    {
        [Column("Id")]
        [Key]
        public long Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("ParentRoleId")]
        public long? ParentRoleId { get; set; } = 0;
    }
}
