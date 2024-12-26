namespace SIFO.Model.Response
{
    public class PharmaciesResponse
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string? MinisterialId { get; set; }
        public long? AddressId { get; set; }
        public string? ASL { get; set; }
        public long? City { get; set; }
        public string CityName { get; set; }
        public long Region { get; set; }
        public string RegionName { get; set; }
        public bool? IsActive { get; set; }
        public string? Address { get; set; }
        public long? PharmacyTypeId { get; set; }
        public string? PharmacyTypeName { get; set; }
        public long? VAT { get; set; }
    }
}
