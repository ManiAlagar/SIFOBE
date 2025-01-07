namespace SIFO.Model.Response
{
    public class TherapeuticPlanDetailResponse
    {
        public long Id { get; set; }
        public long? DrugId { get; set; }
        public long? PharmacyId { get; set; }
        public long? UMRPeriod { get; set; }
        public string Period { get; set; } // Enum('Day', 'Week', 'Month')
        public int? DosageTimes { get; set; }
        public string FrequencyIntake { get; set; } // Enum('Day')
        public long? UMRIntake { get; set; }
        public string DrugDosage { get; set; }
        public DateTime? From { get; set; }
        public DateTime? TakeOn { get; set; }
    }
}
