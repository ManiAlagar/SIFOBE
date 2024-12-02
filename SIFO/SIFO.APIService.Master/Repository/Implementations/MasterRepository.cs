using Microsoft.EntityFrameworkCore;
using SIFO.APIService.Master.Repository.Contracts;
using SIFO.Model.Entity;
using SIFO.Model.Response;

namespace SIFO.APIService.Master.Repository.Implementations
{
    public class MasterRepository : IMasterRepository
    {
        private readonly SIFOContext _context;

        public MasterRepository(SIFOContext context)
        {
            _context = context;
        }

        public async Task<Users> GetUserByEmail(string email)
        {
            try
            {
                var userData = await _context.Users.Where(a => a.Email == email).SingleOrDefaultAsync();
                return userData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Users> IsUserExists(long userId)
        {
            try
            {
                var userData = await _context.Users.Where(a => a.Id == userId).SingleOrDefaultAsync();
                return userData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
