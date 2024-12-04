namespace SIFO.Model.Response
{
    public class StateResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? CountryId { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Iso2 { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool IsActive{ get; set; }
    }
}
