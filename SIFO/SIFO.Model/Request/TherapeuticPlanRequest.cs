namespace SIFO.Model.Request
{
    public class TherapeuticPlanRequest
    {
        public long? Id { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public long HospitalFacilityId { get; set; }
        public List<TherapeuticPlanDetailRequest> therapeuticPlanDetailRequests { get; set; }
    }
}
