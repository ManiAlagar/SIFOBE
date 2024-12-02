using SIFO.APIService.Authentication.Repository.Contracts;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using Microsoft.EntityFrameworkCore;
using SIFO.Model.Response;

namespace SIFO.APIService.Authentication.Repository.Implementations
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly SIFOContext _context;
        public AuthenticationRepository(SIFOContext context)
        {
            _context = context;
        }

        public async Task<Users> IsUserExists(long userId)
        {
            try
            {
                var userData = await _context.Users.Where(a => a.Id == userId).SingleOrDefaultAsync();
                if (userData != null)
                {
                    var roles = await _context.Roles.Where(x => x.Id == userData.RoleId).FirstOrDefaultAsync();
                    var authenticationType = await _context.AuthenticationType.Where(a => a.Id == userData.AuthenticationType).SingleOrDefaultAsync();
                    userData.AuthType = authenticationType.AuthType;
                    userData.RoleName = roles.Name;
                }
                return userData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Users> LoginAsync(LoginRequest request)
        {
            try
            {
                var userData = await _context.Users.Where(x => x.Email == request.Email && x.PasswordHash == request.Password).SingleOrDefaultAsync();
                if (userData != null)
                {
                    var roles = await _context.Roles.Where(x => x.Id == userData.RoleId).FirstOrDefaultAsync();
                    var authenticationType = await _context.AuthenticationType.Where(a => a.Id == userData.AuthenticationType).SingleOrDefaultAsync();
                    userData.AuthType = authenticationType.AuthType;
                    userData.RoleName = roles.Name;
                    userData.ParentRole = await _context.Roles.Where(a => a.ParentRoleId == userData.RoleId).ToListAsync();
                }
                return userData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
        {
            using (var context = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = await _context.Users.Where(x => x.Id == request.UserId && x.IsActive == true).SingleOrDefaultAsync();
                    if (user != null)
                    {
                        user.PasswordHash = request.Password;
                        user.UpdatedDate = DateTime.UtcNow;
                        user.UpdatedBy = request.UserId;
                        await _context.SaveChangesAsync();
                        await _context.Database.CommitTransactionAsync();
                        return true;
                    }
                    return false;
                }
                catch (Exception)
                {
                    await _context.Database.RollbackTransactionAsync();
                    return false;
                }
            }
        }

        public async Task<bool> CreateOtpRequestAsync(OtpRequest otpRequest)
        {
            using (await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var request = await _context.OtpRequests.AddAsync(otpRequest);
                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await _context.Database.RollbackTransactionAsync();
                    return false;
                }
            }
        }
        public async Task<OtpRequest> VerifyRequestAsync(Login2FARequest request)
        {
            try
            {
                var result = await _context.OtpRequests.Where(a => a.OtpCode == request.OtpCode && a.UserId == request.UserId && a.AuthenticationType == request.AuthenticationType && a.AuthenticationFor.ToLower() == request.AuthenticationFor.ToLower()).OrderByDescending(a => a.Id).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<OtpRequest> UpdateOtpRequestAsync(Login2FARequest request)
        {
            using (await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var result = await _context.OtpRequests.Where(a => a.OtpCode == request.OtpCode && a.UserId == request.UserId && a.AuthenticationType == request.AuthenticationType && a.AuthenticationFor.ToLower() == request.AuthenticationFor.ToLower()).OrderByDescending(a => a.Id).FirstOrDefaultAsync();
                    result.UpdatedDate = DateTime.UtcNow;
                    result.UpdatedBy = request.UserId;
                    result.VerifiedDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();
                    return result;
                }
                catch (Exception ex)
                {
                    await _context.Database.RollbackTransactionAsync();
                    throw;
                }
            }
        }

        public async Task<Users> CreateForgotPasswordRequestAsync(ForgotPasswordRequest request)
        {
            using (await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var userData = await _context.Users.Where(x => x.Email == request.Email && x.IsActive == true).SingleOrDefaultAsync();  
                    return userData;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<bool> UpdatePasswordAsync(long userId,string hashedPassword)
        {
            using (await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var userData = await _context.Users.Where(a => a.Id == userId).SingleOrDefaultAsync(); 
                    if(userData != null)
                    {
                        userData.PasswordHash = hashedPassword; 
                        userData.IsTempPassword = true; 
                        userData.UpdatedDate = DateTime.UtcNow; 
                        userData.UpdatedBy = userId; 
                        await _context.SaveChangesAsync(); 
                        await _context.Database.CommitTransactionAsync();
                        return true;
                    } 
                    return false;
                } 
                catch(Exception ex)  
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<PageResponse>> GetPageByUserIdAsync(long userId)
        {
            try
            {
                var result = from user in _context.Users
                             join pagerolepermission in _context.PageRolePermissions on user.RoleId equals pagerolepermission.RoleId
                             join page in _context.Pages on pagerolepermission.PageId equals page.Id
                             where user.Id == userId
                             select new PageResponse
                             {
                                 Id = page.Id,
                                 PageName = page.PageName,
                                 Description = page.Description,
                                 IsActive = page.IsActive,
                                 ParentPageId = page.ParentPageId,
                                 MenuIcon = page.MenuIcon,
                                 PageUrl = page.PageUrl
                             };

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        Task<IEnumerable<PageResponse>> IAuthenticationRepository.GetPageByUserIdAsync(long userId)
        {
            throw new NotImplementedException();
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
    }
}
