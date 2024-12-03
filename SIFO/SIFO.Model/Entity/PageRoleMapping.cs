using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    [Table("pageroleMapping")]
    public class PageRoleMapping
    {
        [Column("Id")]
        [Key]
        public long Id { get; set; }

        [Column("PageId")]
        public long PageId { get; set; }

        [Column("RoleId")]
        public long RoleId { get; set; }
        
        [Column("IsActive")]
        public bool IsActive { get; set; }
    }
}
