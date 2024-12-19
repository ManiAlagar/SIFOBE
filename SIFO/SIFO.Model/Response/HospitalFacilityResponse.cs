namespace SIFO.Model.Response
{
    public class HospitalFacilityResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? ASL { get; set; }
        public long AddressId { get; set; }
        public string? CityName { get; set; }
        public string? RegionName { get; set; }
        public bool IsActive { get; set; }
    }
}
