using SIFO.Model.Request;

namespace SIFO.Model.Response
{
    public class DrugResponse
    {
        public long? Id { get; set; }
        public bool DD { get; set; } = false;
        public bool DPC { get; set; } = false;
        public bool InPharmacy { get; set; } = false;
        public string AIC { get; set; }
        public string ExtendedDescription { get; set; }
        public string CompanyName { get; set; }
        public decimal Price { get; set; }
        public string? ProductType { get; set; }
        public string? Class { get; set; }
        public string? PharmaceuticalForm { get; set; }
        public long? UMR { get; set; }
        public string? PrescriptionType { get; set; }
        public string ProductImage { get; set; }
        public string? TherapeuticIndications { get; set; }
        public string Temperature { get; set; }
        public long? NumberGGAlert { get; set; }
        public int? AlertHours { get; set; }
        public bool IsActive { get; set; }
        public List<DrugRegionResponse> DrugRegionResponse { get; set; }
    }
}
