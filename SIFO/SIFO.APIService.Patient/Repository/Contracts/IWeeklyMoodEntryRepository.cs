using SIFO.Model.Entity;
using SIFO.Model.Response;

namespace SIFO.APIService.Patient.Repository.Contracts
{
    public interface IWeeklyMoodEntryRepository
    {
        public Task<WeeklyMoodEntryResponse> GetAllWeeklyMoodEntryAsync(DateTime? startDate, DateTime? endDate, long patientId);
        public Task<WeeklyMoodEntryResponse> GetWeeklyMoodEntryByIdAsync(long weeklyMoodEntryId);
        public Task<bool> CreateWeeklyMoodEntryAsync(WeeklyMoodEntry entity);
        public Task<bool> UpdateWeeklyMoodEntryAsync(WeeklyMoodEntry entity, long weeklyMoodEntryId);
        public Task<string> DeleteWeeklyMoodEntryAsync(long weeklyMoodEntryId);
        public Task<WeeklyMoodEntry> AlreadyOccurringForTheWeek(DateTime startDate, DateTime endDate, long patientId);
    }
}
