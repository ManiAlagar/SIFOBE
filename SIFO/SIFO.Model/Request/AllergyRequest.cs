namespace SIFO.Model.Request
{
    public class AllergyRequest
    {
        public long Id { get; set; }
        public long patientId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } 
        public long PatientId { get; set; }
    }
}
