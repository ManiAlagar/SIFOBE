namespace SIFO.Model.Response
{
    public class PharmacyResponse
    {
        public long Id { get; set; }
        public long HospitalId { get; set; }
        public string PharmacyName { get; set; }
        public bool IsActive { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long PharmacyTypeId { get; set; }
        public string? PharmacyTypes { get; set; }
        public DateTime? ValidFrom { get; set;} 
        public DateTime? ValidTo { get; set; }


    }
}
