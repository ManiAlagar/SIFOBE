using SIFO.Common.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Entity;
using SIFO.Model.Response;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using SIFO.APIService.User.Repository.Contracts;
using System.Security.Claims;


namespace SIFO.APIService.User.Repository.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly SIFOContext _context;
        private readonly ICommonService _commonService;

        public UserRepository(SIFOContext context, ICommonService commonService)
        {
            _context = context;
            _commonService = commonService;
        }

        //public async Task<string> CreateUserAsync(Users user)
        //{
        //    using (var context = await _context.Database.BeginTransactionAsync()) 
        //    {
        //        try
        //        {

        //            var tokenData = _commonService.GetDataFromToken();
        //            if(user.RoleId.ToString() == tokenData.Result.ParentRoleId)
        //            {
        //                var encryptedPassword = await _commonService.EncryptPassword(user.PasswordHash);
        //                var userData = new Users()
        //                {
        //                    FirstName = user.FirstName,
        //                    LastName = user.LastName,
        //                    Email = user.Email,
        //                    PasswordHash = encryptedPassword,
        //                    PhoneNumber = user.PhoneNumber,
        //                    RoleId = user.RoleId,
        //                    FiscalCode = user.FiscalCode,
        //                    CreatedDate = DateTime.UtcNow,
        //                    CreatedBy = tokenData.Id,
        //                    IsTempPassword = true,
        //                    AuthenticationType = 1
        //                };
        //                var result = await _context.Users.AddAsync(userData);
        //                await _context.SaveChangesAsync();
        //                await _context.Database.CommitTransactionAsync();
        //                return Constants.SUCCESS;
        //            }
        //            else
        //            {
        //                return Constants.INVALID_ROLE;
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            await _context.Database.RollbackTransactionAsync();
        //            throw;
        //        } 
        //    }
        //}
        public async Task<string> CreateUserAsync(Users user)
        {
            try
            {
                var result = await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return Constants.SUCCESS;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<string> CheckIfEmailOrPhoneExists(string email, string phoneNumber, long? userId)
        {
            if (await _context.Users.AsNoTracking().AnyAsync(u => u.Email == email && u.Id != userId && u.IsActive == true))
            {
                return Constants.EMAIL_ALREADY_EXISTS;
            }
            if (await _context.Users.AsNoTracking().AnyAsync(u => u.PhoneNumber == phoneNumber && u.Id != userId && u.IsActive == true))
            {
                return Constants.PHONE_ALREADY_EXISTS;
            }
            return Constants.SUCCESS;
        }

        public async Task<UserResponse> GetUserById(long? id, long roleId, string parentRoleId)
        {
            try
            {
                var tokenData = _commonService.GetDataFromToken();

                var userData = from user in _context.Users
                               join role in _context.Roles on user.RoleId equals role.Id
                               where parentRoleId.Contains(user.RoleId.ToString()) && user.RoleId == roleId && user.Id == id
                               select new UserResponse
                               {
                                   Id = user.Id,
                                   FirstName = user.FirstName,
                                   LastName = user.LastName,
                                   Email = user.Email,
                                   PhoneNumber = user.PhoneNumber,
                                   RoleId = user.RoleId,
                                   RoleName = role.Name,
                                   FiscalCode = user.FiscalCode,
                                   AuthenticationType = 1,
                                   AuthenticationName = "Email",
                                   ProfileImg = user.ProfileImg,
                                   IsActive = user.IsActive,
                                   
                               };

                return await userData.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> DeleteUserById(long id, long roleId, string parentRoleId)
        {

            using (var context = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var userToDelete = await (from user in _context.Users
                                              join role in _context.Roles on user.RoleId equals role.Id
                                              where parentRoleId.Contains(user.RoleId.ToString()) && user.RoleId == roleId && user.Id == id
                                              select user).FirstOrDefaultAsync();

                    if (userToDelete != null)
                    {
                        var otpDelete = await _context.OtpRequests.Where(x => x.UserId == userToDelete.Id).ToListAsync();
                        _context.OtpRequests.RemoveRange(otpDelete);
                        await _context.SaveChangesAsync();
                        var userSession = await _context.UserSessionManagements.Where(x => x.UserId == userToDelete.Id).ToListAsync();
                        _context.UserSessionManagements.RemoveRange(userSession);
                        await _context.SaveChangesAsync();
                        _context.Users.Remove(userToDelete);
                        await _context.SaveChangesAsync();
                        await _context.Database.CommitTransactionAsync();
                        return Constants.SUCCESS;
                    }

                    return Constants.NOT_FOUND;
                }
                catch (Exception ex)
                {
                    await _context.Database.RollbackTransactionAsync();
                    throw;
                }
            }
        }


        public async Task<string> GetPasswordByUserId(long id)
        {

           return  await _context.Users.Where(a => a.Id == id).Select(a => a.PasswordHash).FirstOrDefaultAsync();
        }

        //public async Task<string> UpdateUserAsync(Users user, long? existingUserParentId, string parentRoleId)
        //{
        //    try
        //    {
        //        if (existingUserParentId.ToString() == parentRoleId)
        //        {
        //            _context.Users.Update(user);
        //            await _context.SaveChangesAsync();
        //            return Constants.SUCCESS;
        //        }
        //        return Constants.INVALID_ROLE;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
        public async Task<string> UpdateUserAsync(Users user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return Constants.SUCCESS;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<PagedResponse<UserResponse>> GetAllUsersAsync(int pageIndex, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, long? roleId, string parentRoleId)
        {


            var query = from user in _context.Users
                        join role in _context.Roles on user.RoleId equals role.Id
                        join authtype in _context.AuthenticationType on user.AuthenticationType equals authtype.Id
                        where parentRoleId.Contains(user.RoleId.ToString()) && (user.RoleId == roleId || roleId == null)
                        select new UserResponse
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            RoleId = user.RoleId,
                            RoleName = role.Name,
                            FiscalCode = user.FiscalCode,
                            AuthenticationType = user.AuthenticationType,
                            AuthenticationName = authtype.AuthType,
                            ProfileImg = user.ProfileImg,
                            IsActive = user.IsActive
                        };
            var sqlQuery = query.ToQueryString();

            var count = query.Count();
            PagedResponse<UserResponse> pagedResponse = new PagedResponse<UserResponse>();

            if (isAll)
            {
                var result = query.Where(a => a.IsActive == true).ToList();

                pagedResponse.Result = result;
                pagedResponse.TotalCount = count;
                pagedResponse.TotalPages = 1;
                pagedResponse.CurrentPage = 1;
                return pagedResponse;
            }

            string orderByExpression = $"{sortColumn} {sortDirection}";

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(u => u.FirstName.Contains(filter) || u.LastName.Contains(filter) || u.Email.Contains(filter));
                count = query.Count();
            }

            query = query.OrderBy(orderByExpression).Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var pagedResult = await query.ToListAsync();

            pagedResponse.Result = pagedResult.AsEnumerable();
            pagedResponse.TotalCount = count;
            pagedResponse.TotalPages = (int)Math.Ceiling((pagedResponse.TotalCount ?? 0) / (double)pageSize);
            pagedResponse.CurrentPage = pageIndex;

            return pagedResponse;
        }

        public async Task<Role> GetRoleById(long? id)
        {
            try
            {
                var result = await _context.Roles.Where(a => a.Id == id).SingleOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}