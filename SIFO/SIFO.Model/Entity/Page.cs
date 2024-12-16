using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    [Table("pages")]
    public class Page
    {
        [Column("Id")]
        [Key]
        public long Id { get; set; }

        [Column("PageName")]
        public string PageName { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("ParentPageId")]
        public long? ParentPageId { get; set; }

        [Column("MenuIcon")]
        public string MenuIcon { get; set; }

        [Column("PageUrl")]
        public string PageUrl { get; set; }
        [Column("EventName")] 
        public string EventName { get; set; }

        [Column("fk_userRoles")]
        public long? userRoleId { get; set; }
    }
}
