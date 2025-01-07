using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Patient.Service.Contracts
{
    public interface IWeeklyMoodEntryService
    {
        public Task<ApiResponse<Dictionary<string, WeeklyMoodEntryResponse>>> GetAllWeeklyMoodEntryAsync(DateTime? startDate, DateTime? endDate, long patientId);
        public Task<ApiResponse<WeeklyMoodEntryResponse>> GetWeeklyMoodEntryByIdAsync(long weeklyMoodEntryId);
        public Task<ApiResponse<string>> CreateWeeklyMoodEntryAsync(WeeklyMoodEntryRequest request);
        public Task<ApiResponse<string>> UpdateWeeklyMoodEntryAsync(WeeklyMoodEntryRequest request, long weeklyMoodEntryId);
        public Task<ApiResponse<string>> DeleteWeeklyMoodEntryAsync(long weeklyMoodEntryId);
    }
}
