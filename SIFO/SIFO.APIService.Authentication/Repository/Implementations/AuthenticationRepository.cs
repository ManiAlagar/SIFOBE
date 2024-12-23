﻿using SIFO.APIService.Authentication.Repository.Contracts;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using Microsoft.EntityFrameworkCore;
using SIFO.Model.Response;
using System.Linq;
using SIFO.Model.Constant;

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
                    userData.ParentRole = await _context.RolePermissions.Where(rp => rp.RoleId == userData.RoleId && rp.IsActive == true).Select(rp => rp.AllowedRoleId).ToListAsync();
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
                var userData = await (from user in _context.Users
                                      join role in _context.Roles on user.RoleId equals role.Id
                                      join authType in _context.AuthenticationType on user.AuthenticationType equals authType.Id
                                      where user.Email == request.Email && user.PasswordHash == request.Password && user.IsActive == true
                                      select new Users
                                      {
                                          Id=user.Id,
                                          Email= user.Email,
                                          PasswordHash = user.PasswordHash,
                                          RoleId=  user.RoleId,
                                         AuthenticationType= user.AuthenticationType,
                                          AuthType = authType.AuthType,
                                          RoleName = role.Name,
                                          ParentRole = _context.RolePermissions
                                                          .Where(rp => rp.RoleId == user.RoleId && rp.IsActive == true)
                                                          .Select(rp => rp.AllowedRoleId)
                                                          .ToList()
                                      }).FirstOrDefaultAsync();

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

        public async Task<bool> UpdatePasswordAsync(long userId,string hashedPassword,bool isTemp)
        {
            using (await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var userData = await _context.Users.Where(a => a.Id == userId).SingleOrDefaultAsync(); 
                    if(userData != null)
                    {
                        userData.PasswordHash = hashedPassword; 
                        userData.IsTempPassword = isTemp; 
                        userData.UpdatedDate = DateTime.UtcNow; 
                        userData.UpdatedBy = userId;
                        userData.PswdUpdatedAt = DateTime.UtcNow;
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

        public async Task<List<PageResponse>> GetPageByUserIdAsync(long userId)
        {
            try
            {
                var result = from user in _context.Users
                             join pagerolepermission in _context.PageRoleMapping on user.RoleId equals pagerolepermission.RoleId
                             join page in _context.Pages on pagerolepermission.PageId equals page.Id
                             where user.Id == userId && pagerolepermission.IsActive == true
                             select new PageResponse
                             {
                                 Id = page.Id,
                                 PageName = page.PageName,
                                 IsActive = page.IsActive.Value,
                                 ParentPageId = page.ParentPageId,
                                 MenuIcon = page.MenuIcon,
                                 PageUrl = page.PageUrl,
                                 userRoleId = page.userRoleId,
                                 EventName = page.EventName
                             };

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
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

        public async Task<List<RoleResponse>> CreatePermission(long roleId)
        {
            try
            {
                var userRole = await _context.Roles.Where(a => a.ParentRoleId == roleId)
                .Select(a => new RoleResponse
                {
                    Id = a.Id,
                    Name = a.Name
                }).ToListAsync();
                return userRole;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> LogoutAsync(long id)
        {
            try
            {
                var result = await _context.UserSessionManagements.FirstOrDefaultAsync(x => x.UserId == id);
                result.DtLogout = DateTime.UtcNow;

                if (result != null)
                {
                    _context.Update(result);
                    _context.SaveChanges();
                    return Constants.SUCCESS;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CreateUserSessionManagementAsync(UserSessionManagement userSessionManagement)
        {
            try
            {
                await _context.UserSessionManagements.AddAsync(userSessionManagement);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
