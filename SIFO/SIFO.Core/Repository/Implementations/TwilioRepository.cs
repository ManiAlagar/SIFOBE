using SIFO.Model.Entity;
using Microsoft.EntityFrameworkCore;
using SIFO.Core.Repository.Contracts;

namespace SIFO.Core.Repository.Implementations
{
    public class TwilioRepository : ITwilioRepository
    {
        private readonly SIFOContext _context;

        public TwilioRepository(SIFOContext context)
        {
            _context = context;
        }

        public async Task<string?> GetServiceIdbyUserIDAsync(long userId)
        {
            try
            {
                string response = await _context.Users.Where(x => x.Id == userId).Select(x => x.UserSid).FirstOrDefaultAsync();
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> CreateOrUpdateServiceIdAsync(long userId, string userSid)
        {
            try
            {
                var response = await _context.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
                if (response is not null)
                {
                    response.UserSid = userSid;
                    response.UpdatedDate = DateTime.UtcNow;
                    response.UpdatedBy = userId; 
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
