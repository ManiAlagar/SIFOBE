using SIFO.APIService.Authentication.Repository.Contracts;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using Microsoft.EntityFrameworkCore;
using SIFO.Model.Response;
using Microsoft.Extensions.Configuration;
using SIFO.Model.Constant;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SIFO.APIService.Authentication.Repository.Implementations
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly SIFOContext _context;
        public AuthenticationRepository(SIFOContext context)
        {
            _context = context;
        }

        public async Task<UserResponse> GetUserContactInfo(long userId)
        {
            try
            {
                var query = from user in _context.Users
                            join role in _context.Roles
                            on user.RoleId equals role.Id
                            where user.Id == userId
                            select new UserResponse
                            {
                                UserId = user.Id,
                                UserName = user.FirstName,
                                Email = user.Email,
                                IsActive = user.IsActive,
                                Role = role.Name,
                            };

                return await(query).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<User> LoginAsync(LoginRequest request)
        {
            try
            {
                var userData = await _context.Users.Where(x => x.Email == request.Email && x.PasswordHash == request.Password).SingleOrDefaultAsync();
                var roles = await _context.Roles.Where(x => x.Id == userData.RoleId).FirstOrDefaultAsync();
                userData.RoleName = roles.Name;
                return userData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            using (var context = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = await _context.Users.Where(x => x.Id == changePasswordRequest.Id && x.IsActive == true).SingleOrDefaultAsync();
                    if (user != null)
                    {
                        user.PasswordHash = changePasswordRequest.Password;
                        user.UpdatedDate = DateTime.UtcNow;
                        user.UpdatedBy = changePasswordRequest.Id;
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
    }
}
