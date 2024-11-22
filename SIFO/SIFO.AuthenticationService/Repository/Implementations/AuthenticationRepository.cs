using SIFO.AuthenticationService.Repository.Contracts;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using Microsoft.EntityFrameworkCore;
using SIFO.Model.Response;

namespace SIFO.AuthenticationService.Repository.Implementations
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly SIFOContext _context;
        public AuthenticationRepository(SIFOContext context)
        {
            _context = context;
        }

        public Task<UserContactInfo> GetUserContactInfo(long userId)
        {
            throw new NotImplementedException();
        }

        public async Task<User> LoginAsync(LoginRequest request)
        {
			try
			{
                var userData = await _context.Users.Where(x => x.Email == request.Email && x.PasswordHash == request.Password).SingleOrDefaultAsync();
                return userData;
            }
			catch (Exception ex)
			{ 
				throw;
			}
        }
    }
}
