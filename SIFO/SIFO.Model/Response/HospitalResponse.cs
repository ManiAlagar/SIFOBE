using System.ComponentModel.DataAnnotations.Schema;

namespace SIFO.Model.Response
{
    public class HospitalResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? AddressId { get; set; }
        public string? ASL { get; set; }
        public string Province { get; set; }
        public string PhoneNumber { get; set; }
        public bool? IsActive { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public string? CAB { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<ContactResponse>? Contacts { get; set; }
        public List<PharmacyResponse>? Pharmacies{ get; set; }
        
    }
}
