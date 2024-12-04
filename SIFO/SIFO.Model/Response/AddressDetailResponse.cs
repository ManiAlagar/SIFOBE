namespace SIFO.Model.Response
{
    public class AddressDetailResponse
    {
        public long Id { get; set; }
        public string? Address { get; set; }
        public long? CityId { get; set; }
        public string? CityName { get; set; }
        public long? Region { get; set; }
        public long? CountryId { get; set; }
        public string? CountryName { get; set; }
        public long? Zipcode { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
