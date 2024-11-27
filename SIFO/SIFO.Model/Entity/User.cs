using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Entity
{
    public class User
    {
        public long Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileURL { get; set; }
        public string? ZipCode { get; set; }
        public long? RoleId { get; set; }
       // public long? AddressDetailId { get; set; }
        public string? FiscalCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool? IsActive { get; set; } = true;
        [NotMapped]
        public string? RoleName { get; set; }
        public bool? IsTempPassword { get; set; } = false;
        public long? AuthenticationType { get; set; }
        [NotMapped]
        public string? AuthType { get; set; }
    }
}
