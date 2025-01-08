namespace SIFO.Model.Request
{
    public class HospitalRequest
    {
        public string HospitalName { get; set; }
        public long Region { get; set; }
        public long AddressId { get; set; }
        public long? CountryId { get; set; }
        public string Address { get; set; }
        public long? ZipCode { get; set; }
        public long? City { get; set; }
        public bool IsActive { get; set; }
        public string ASL { get; set; }
        public string PhoneNumber { get; set; }
        public string Province { get; set; }
        public string CAB { get; set; }
        public List<ContactRequest> Contact { get; set; }
        public List<HospitalPharmacyRequest> Pharmacy { get; set; }
    }
}
