namespace SIFO.Model.Request
{
    public class HospitalFacilityRequest
    {
        public string HospitalFacilityName { get; set; }
        public long Region { get; set; }
        public long AddressId { get; set; }
        public string? CountryCode { get; set; }
        public string Address { get; set; }
        public long? ZipCode { get; set; }
        public bool IsActive { get; set; }
        public string ASL { get; set; }
        public string PhoneNumber { get; set; }
        public long City { get; set; }
        public string Province { get; set; }
        public string CAP { get; set; }
        public List<ContactRequest> Contact { get; set; }
        public List<long> PharmacyIds { get; set; }
    }
}
