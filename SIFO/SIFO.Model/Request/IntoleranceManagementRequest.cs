namespace SIFO.Model.Request
{
    public class IntoleranceManagementRequest
    {
        public long? Id { get; set; }
        public long PatientId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
