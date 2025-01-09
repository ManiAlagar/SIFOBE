using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SIFO.Model.Entity
{
    [Table("patients")]
    public class Patients
    {
        [Key]
        [Column("id")]
        public long? Id { get; set; }

        [Column("code")]
        public string Code { get; set; }

        [Column("lastName")]
        public string? LastName { get; set; }

        [Column("firstName")]
        public string? FirstName { get; set; }

        [Column("fiscalCode")]
        public string? FiscalCode { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("deliveryMethod")]
        public string? DeliveryMethod { get; set; }

        [Column("deliveryPharmacyId")]
        public long? DeliveryPharmacyId { get; set; }

        [Column("addressId")]
        public long? AddressId { get; set; }

        [Column("notificationModeId")]
        public long? NotificationModeId { get; set; }

        [Column("smsReminder")]
        public bool? SmsReminder { get; set; }

        [Column("reminderApp")]
        public bool? ReminderApp { get; set; }

        [Column("consentPersonalData")]
        public bool? ConsentPersonalData { get; set; }

        [Column("consentSensitiveData")]
        public bool? ConsentSensitiveData { get; set; }

        [Column("consentDataProfiling")]
        public bool? ConsentDataProfiling { get; set; }

        [Column("consentThirdPartyMarketing")]
        public bool? ConsentThirdPartyMarketing { get; set; }

        [Column("createdBy")]
        public long CreatedBy { get; set; }

        [Column("createdDate")]
        public DateTime CreatedDate { get; set; }

        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }

        [Column("updatedDate")]
        public DateTime? UpdatedDate { get; set; }

        [Column("isActive")]
        public bool IsActive { get; set; }

        [Column("IsVerified")] 
        public bool IsVerified { get; set; }

        [Column("password")]
        public string? Password { get; set; }

        [Column("pswdUpdatedAt")]
        public DateTime? PswdUpdatedAt { get; set; }

        [Column("roleId")]
        public long? RoleId { get; set; }

        [Column("authenticationType")]
        public long? AuthenticationType { get; set; }
    }
}
