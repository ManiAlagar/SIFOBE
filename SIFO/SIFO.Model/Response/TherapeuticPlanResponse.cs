namespace SIFO.Model.Response
{
    public class TherapeuticPlanResponse
    {
        public long Id { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public long HospitalFacilityId { get; set; }
        public string HospitalFacilityName { get; set; }
        public long? TherapeuticPlanDetailsId { get; set; }
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public List<TherapeuticPlanDetailResponse> therapeuticPlanDetailResponses { get; set; }
    }
}
