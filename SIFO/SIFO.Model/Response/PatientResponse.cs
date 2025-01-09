using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SIFO.Model.Response
{
    public class PatientResponse
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string FiscalCode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string DeliveryMethod { get; set; }
        public long? DeliveryPharmacyId { get; set; }
        public string? DeliveryPharmacyName { get; set; }
        public long? AddressId { get; set; }
        public string? Address { get; set; }
        public long? CityId{ get; set; }
        public string? City{ get; set; }
        public long? StateId{ get; set; }
        public string? State{ get; set; }
        public long? CountryId{ get; set; }
        public string? Country{ get; set; }
        public long NotificationModeId { get; set; }
        public bool SmsReminder { get; set; }
        public bool ReminderApp { get; set; }
        public bool ConsentPersonalData { get; set; }
        public bool ConsentSensitiveData { get; set; }
        public bool? ConsentDataProfiling { get; set; }
        public bool? ConsentThirdPartyMarketing { get; set; }
        public long? RoleId { get; set; }
    }
}
