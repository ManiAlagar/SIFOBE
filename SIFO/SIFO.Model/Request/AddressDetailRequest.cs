namespace SIFO.Model.Request
{
    public class AddressDetailRequest
    {
        public long Id { get; set; }
        public string? address { get; set; } 
        public long? CityId { get; set; }
        public long? Region { get; set; }
        public long? CountryId { get; set; }
        public long? Zipcode { get; set; }
        public bool IsActive { get; set; }
    }
}
