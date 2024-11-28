using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    [Table("pagerolepermissions")]
    public class PageRolePermission
    {
        [Column("Id")]
        [Key]
        public long Id { get; set; }

        [Column("PageId")]
        public long PageId { get; set; }

        [Column("RoleId")]
        public long RoleId { get; set; }

        [Column("PermissionsId")]
        public long? PermissionsId { get; set; }
    }
}
