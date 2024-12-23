using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    [Table("tbl_label")]
    public class Labels
    {
        [Key]
        [Column("id")]
        public long Id{ get; set; }

        [Column("fkVar")] 
        public string FkVar { get; set;}

        [Column("language")]
        public string Language { get; set;}

        [Column("theLabel")] 
        public string Label { get; set;}    
    }
}
