namespace SIFO.Model.Response
{
    public class PharmacyDetailResponse
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
        public List<ContactResponse>? Contacts { get; set; }
        public Dictionary<string, List<CalendarResponse>>? Calendar { get; set; }
    }
}
