namespace SIFO.Model.Request
{
    public class StateRequest
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long CountryId { get; set; }
        public string? CountryCode { get; set; } = string.Empty;
        public string? Iso2 { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool IsActive{ get; set; }
    }
}
