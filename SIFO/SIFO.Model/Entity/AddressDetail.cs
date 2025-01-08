using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    [Table("addressdetails")]
    public class AddressDetail
    {
        [Column("Id")]
        [Key]
        public long Id { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [Column("CityId")]
        public long? CityId { get; set; }

        [Column("Region")]
        public long? Region { get; set; }

        [Column("CountryId")]
        public long? CountryId { get; set; }

        [Column("Zipcode")]
        public long? Zipcode { get; set; }

        [Column("IsActive")]
        public bool? IsActive { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }
    }
}
