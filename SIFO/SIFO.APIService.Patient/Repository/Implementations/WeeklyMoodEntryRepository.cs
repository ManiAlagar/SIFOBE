using SIFO.Model.Entity;
using SIFO.Model.Response;
using SIFO.Model.Constant;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using SIFO.APIService.Patient.Repository.Contracts;

namespace SIFO.APIService.Patient.Repository.Implementations
{
    public class WeeklyMoodEntryRepository : IWeeklyMoodEntryRepository
    {
        private readonly SIFOContext _context;

        public WeeklyMoodEntryRepository(SIFOContext context)
        {
            _context = context;
        }

        public async Task<WeeklyMoodEntryResponse> GetAllWeeklyMoodEntryAsync(DateTime? startDate, DateTime? endDate, long patientId)
        {
            try
            {
                var response = await (from result in _context.WeeklyMoodEntries
                                      where result.WeekStartDate.Value.Date >= startDate.Value.Date && result.WeekEndDate.Value.Date <= endDate.Value.Date
                                      && result.PatientId == patientId
                                      orderby result.Id
                                      select new WeeklyMoodEntryResponse
                                      {
                                          Id = result.Id,
                                          ColorCode = result.ColorCode,
                                          ImagePath = result.ImagePath,
                                          WeekStartDate = result.WeekStartDate,
                                          WeekEndDate = result.WeekEndDate,
                                          IsActive = result.IsActive,
                                          PatientId = result.PatientId
                                      }).LastOrDefaultAsync();
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<WeeklyMoodEntryResponse> GetWeeklyMoodEntryByIdAsync(long weeklyMoodEntryId)
        {
            try
            {
                var response = await (from result in _context.WeeklyMoodEntries
                                      select new WeeklyMoodEntryResponse
                                      {
                                          Id = result.Id,
                                          ColorCode = result.ColorCode,
                                          ImagePath = result.ImagePath,
                                          WeekStartDate = result.WeekStartDate,
                                          WeekEndDate = result.WeekEndDate,
                                          IsActive = result.IsActive,
                                      }).FirstOrDefaultAsync(); ;
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> CreateWeeklyMoodEntryAsync(WeeklyMoodEntry entity)
        {
            try
            {
                var result = await _context.WeeklyMoodEntries.AddAsync(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> UpdateWeeklyMoodEntryAsync(WeeklyMoodEntry entity, long weeklyMoodEntryId)
        {
            try
            {
                var result = await _context.WeeklyMoodEntries.Where(a => a.Id == weeklyMoodEntryId).SingleOrDefaultAsync();
                if (result is not null)
                {
                    result.ColorCode = entity.ColorCode;
                    result.ImagePath = entity.ImagePath;
                    result.PatientId = entity.PatientId;
                    result.WeekStartDate = entity.WeekStartDate;
                    result.WeekEndDate = entity.WeekEndDate;
                    result.IsActive = entity.IsActive;
                    result.UpdatedDate = entity.UpdatedDate;
                    result.UpdatedBy = entity.UpdatedBy;
                    _context.WeeklyMoodEntries.Update(result);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> DeleteWeeklyMoodEntryAsync(long weeklyMoodEntryId)
        {
            try
            {
                var entity = await _context.WeeklyMoodEntries.Where(x => x.Id == weeklyMoodEntryId).SingleOrDefaultAsync();
                if (entity != null)
                {
                    _context.WeeklyMoodEntries.Remove(entity);
                    await _context.SaveChangesAsync();
                    return Constants.SUCCESS;
                }
                return Constants.NOT_FOUND;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is MySqlConnector.MySqlException mysqlEx && mysqlEx.Number == Constants.DATA_DEPENDENCY_CODE)
                {
                    return Constants.DATA_DEPENDENCY_ERROR_MESSAGE;
                }
                throw;
            }
        }

        public async Task<WeeklyMoodEntry> AlreadyOccurringForTheWeek(DateTime startDate, DateTime endDate, long patientId)
        {
            try
            {
                var result = await _context.WeeklyMoodEntries.Where(a => a.WeekStartDate.Value.Date == startDate.Date && a.WeekEndDate.Value.Date == endDate.Date && a.PatientId == patientId).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
