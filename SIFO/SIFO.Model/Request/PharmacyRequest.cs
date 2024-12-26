namespace SIFO.Model.Request
{
    public class PharmacyRequest
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string ASL { get; set; }
        public long Region { get; set; }
        public long AddressId { get; set; }
        public string Address { get; set; }
        public long? City { get; set; }
        public string CAP { get; set; }
        public string Province { get; set; }
        public string PhoneNumber { get; set; }
        public string? MinisterialId { get; set; }
        public long? CountryId { get; set; }
        public long? ZipCode { get; set; }
        public long PharmacyTypeId { get; set; }
        public bool IsActive { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public long? VAT { get; set; }
        public List<ContactRequest> Contact { get; set; }
    }
}
