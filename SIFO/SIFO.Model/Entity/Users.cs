using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    public class Users
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImg { get; set; }
        public long RoleId { get; set; }
       // public long? AddressDetailId { get; set; }
        public string? FiscalCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public bool? IsActive { get; set; } = true;
        public bool? IsTempPassword { get; set; } = false;
        public DateTime? PswdUpdatedAt { get; set; }
        public long? AuthenticationType { get; set; }
        public long CountryId { get; set; }

        [NotMapped]
        public string? RoleName { get; set; }
        [NotMapped]
        public List<long>? PharmacyIds { get; set; }
        [NotMapped]
        public List<long>? HospitalIds { get; set; }

        [NotMapped]
        public string? AuthType { get; set; }
        public string? UserSid { get; set; }

        [NotMapped] 
        public List<long> ParentRole { get; set; }
    }
}
