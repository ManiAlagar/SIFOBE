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
        public int? CountryId { get; set; }

        [Column("Country_Code")]
        public string? CountryCode { get; set; }

        [Column("Fips_Code")]
        public string? FipsCode { get; set; }

        [Column("Iso2")]
        public string? Iso2 { get; set; }

        [Column("Type")]
        public string? Type { get; set; }

        [Column("Latitude")]
        public decimal? Latitude { get; set; }

        [Column("Longitude")]
        public decimal? Longitude { get; set; }

        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }

        [Column("Flag")]
        public bool Flag { get; set; }

        [Column("WikiDataId")]
        public string? WikiDataId { get; set; }
    }
}
