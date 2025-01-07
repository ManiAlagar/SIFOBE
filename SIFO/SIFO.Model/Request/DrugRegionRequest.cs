namespace SIFO.Model.Request
{
    public class DrugRegionRequest
    {
        public long? DrugsRegionsId { get; set; }
        public long RegionId { get; set; }
        public string DrugType { get; set; }
        public long DrugId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool? IsNew { get; set; } = false;
        public bool? IsDeleted { get; set; } = false;
    }
}
