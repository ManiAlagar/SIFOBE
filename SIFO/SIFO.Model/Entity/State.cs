using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    [Table("states")]
    public class State
    {
        [Column("Id")]
        [Key]
        public long Id { get; set; }

        [Column("Name")]
        public string? Name { get; set; }

        [Column("Country_Id")]
        public long? CountryId { get; set; }

        [Column("Country_Code")]
        public string? CountryCode { get; set; }

        [Column("Iso2")]
        public string? Iso2 { get; set; }

        [Column("Latitude")]
        public decimal? Latitude { get; set; }

        [Column("Longitude")]
        public decimal? Longitude { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("createdDate")]
        public DateTime? createdDate { get; set; }
    }
}
