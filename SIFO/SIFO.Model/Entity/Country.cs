using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    [Table("countries")]
    public class Country
    {
        [Column("Id")]
        [Key]
        public long Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Iso3")]
        public string? Iso3 { get; set; }

        [Column("Iso2")]
        public string? Iso2 { get; set; }

        [Column("PhoneCode")]
        public string? PhoneCode { get; set; }

        [Column("Currency_Symbol")]
        public string? Currencysymbol { get; set; }

        [Column("Timezones")]
        public string? Timezones { get; set; }

        [Column("Latitude")]
        public decimal? Latitude { get; set; }

        [Column("Longitude")]
        public decimal? Longitude { get; set; }

        [Column("EmojiU")]
        public string? EmojiU { get; set; }

        [Column("createdDate")]
        public DateTime? createdDate { get; set; }
    }
}
