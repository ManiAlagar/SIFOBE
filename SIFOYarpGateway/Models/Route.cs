using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIFOYarpGateway.Models
{
    [Table("routes")]
    public class Routes
    {
        [Key]
        public int Id { get; set; }
        [Column("route")]
        public string Route { get; set; }
        [Column("cluster")]
        public string Cluster { get; set; }
        [Column("pathpattern")]
        public string PathPattern { get; set; }
    }
}
