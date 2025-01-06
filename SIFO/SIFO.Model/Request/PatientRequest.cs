namespace SIFO.Model.Request
{
    public class PatientRequest
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string FiscalCode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public long DeliveryMethodId { get; set; }
        public long? DeliveryPharmacyId { get; set; }
        public long? AddressId { get; set; }
        public long NotificationModeId { get; set; }
        public bool SmsReminder { get; set; }
        public bool ReminderApp { get; set; }
        public bool ConsentPersonalData { get; set; }
        public bool ConsentSensitiveData { get; set; }
        public bool? ConsentDataProfiling { get; set; }
        public bool? ConsentThirdPartyMarketing { get; set; }
        public bool IsActive { get; set; }
    }
}
