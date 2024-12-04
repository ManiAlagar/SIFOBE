using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    [Table("cities")]
    public class City
    {
        [Column("Id")]
        [Key]
        public long Id { get; set; }

        [Column("Name")]
        public string? Name { get; set; }

        [Column("StateId")]
        public long? StateId { get; set; }

        [Column("StateCode")]
        public string? StateCode { get; set; }

        [Column("CountryId")]
        public long? CountryId { get; set; }

        [Column("CountryCode")]
        public string? CountryCode { get; set; }

        [Column("Latitude")]
        public decimal? Latitude { get; set; }

        [Column("Longitude")]
        public decimal? Longitude { get; set; }

        [Column("createdDate")]
        public DateTime? createdDate { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }
    }
}
