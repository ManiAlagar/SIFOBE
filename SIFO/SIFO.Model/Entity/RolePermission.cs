using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    [Table("RolePermissions")]
    public class RolePermission
    {
        [Key]
        public long RolePermissionId { get; set; }
        [Column("RoleId")]
        public long RoleId { get; set; }
        [Column("AllowedRoleId")]
        public long AllowedRoleId { get; set; }
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

    }

}
