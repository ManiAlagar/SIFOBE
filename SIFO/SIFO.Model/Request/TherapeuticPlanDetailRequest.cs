using static SIFO.Model.Constant.Constants;

namespace SIFO.Model.Request
{
    public class TherapeuticPlanDetailRequest
    {
        public long DrugId { get; set; }
        public long? PharmacyId { get; set; }
        //public long? TherapeuticPlanId { get; set; }
        public long? UMRPeriod { get; set; }
        public PeriodTypes Period { get; set; } // Enum('Day', 'Week', 'Month')
        public int? DosageTimes { get; set; }
        public FrequencyIntake FrequencyIntake { get; set; } // Enum('Day')
        public string TypeIntake { get; set; } // Enum('Day')
        public long? UMRIntake { get; set; }
        public string DrugDosage { get; set; }
        public DateTime? From { get; set; }
        public DateTime? TakeOn { get; set; }
        //public List<TherapeuticPlanRequest> therapeuticPlanRequests { get; set; }
    }
}
