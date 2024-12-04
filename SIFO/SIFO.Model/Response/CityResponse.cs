namespace SIFO.Model.Response
{
    public class CityResponse
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public long? StateId { get; set; }
        public string? StateCode { get; set; }
        public string? StateName { get; set; }
        public string? CountryName { get; set; }
        public long? CountryId { get; set; }
        public string? CountryCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
