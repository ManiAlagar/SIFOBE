using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    [Table("Contacts")]
    public class Contact
    {

        [Column("Id")]
        [Key]
        public long Id { get; set; }

        [Column("HospitalStructureId")]
        public long HospitalId { get; set; }

        [Column("ContactName")]
        public string ContactName { get; set; }

        [Column("ContactSurname")]
        public string ContactSurname { get; set; }

        [Column("Role")]
        public string Role { get; set; }

        [Column("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("CreatedBy")]
        public long? CreatedBy { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Column("UpdatedBy")]
        public long? UpdatedBy { get; set; }

        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }

    }
}
