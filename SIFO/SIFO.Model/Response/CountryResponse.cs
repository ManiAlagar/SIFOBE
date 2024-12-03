namespace SIFO.Model.Response
{
    public class CountryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Iso3 { get; set; } 
        public string Iso2 { get; set; } 
        public string PhoneCode { get; set; } 
        public string Timezones { get; set; }
        public decimal? Latitude { get; set; } 
        public decimal? Longitude { get; set; }
        public string EmojiU { get; set; } 
        public bool IsActive{ get; set;}
    }
}
