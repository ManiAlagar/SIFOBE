﻿using Microsoft.EntityFrameworkCore;
using SIFO.APIService.User.Repository.Contracts;
using SIFO.Common.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Entity;
using SIFO.Model.Response;
using System.Linq.Dynamic.Core;


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
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var roleName = await GetRoleById(user.RoleId);
                    var result = await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();
                    if(roleName.Name == Constants.ROLE_HOSPITAL_PHARMACY_SUPERVISOR || roleName.Name == Constants.ROLE_HOSPITAL_PHARMACY_OPERATOR)
                    {
                        if (user.PharmacyIds != null && user.PharmacyIds.Any())
                        {
                           //_context
                            foreach (var pharmacyId in user.PharmacyIds)
                            {
                                var isInsertSuccess = await InsertUserPharmacyMapping(result.Entity.Id, pharmacyId);
                                if (isInsertSuccess != Constants.SUCCESS)
                                {
                                    await transaction.RollbackAsync();
                                    return isInsertSuccess;
                                }
                            }
                        }

                    }
                    await transaction.CommitAsync();
                    return Constants.SUCCESS;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Return error message instead of throwing an exception
                    return $"An error occurred while creating the user: {ex.Message}";
                }
            }
        }

        public async Task<string> InsertUserPharmacyMapping(long userId, long pharmacyId)
        {
            try
            {
                var mappingData = new UserPharmacyMapping
                {
                    userId = userId,
                    PharmacyId = pharmacyId
                };
                await _context.UserPharmacyMappings.AddAsync(mappingData);
                await _context.SaveChangesAsync();
                return Constants.SUCCESS;
            }
            catch (Exception ex)
            {
                return $"An error occurred while inserting the user-pharmacy mapping: {ex.Message}";
            }
        }


        public async Task<string> CheckIfEmailOrPhoneExists(string email, string phoneNumber, long? userId)
        {
            var userExists = await _context.Users.AsNoTracking()
                .Where(u => (u.Email == email || u.PhoneNumber == phoneNumber) && u.Id != userId)
                .Select(u => new { u.Email, u.PhoneNumber })
                .FirstOrDefaultAsync();

            if (userExists != null)
            {
                if (userExists.Email == email)
                {
                    return Constants.EMAIL_ALREADY_EXISTS;
                }

                if (userExists.PhoneNumber == phoneNumber)
                {
                    return Constants.PHONE_ALREADY_EXISTS;
                }
            }

            return Constants.SUCCESS;
        }

        public async Task<UserResponse> GetUserById(long? id, long roleId, string parentRoleId)
        {
            try
            {


                var userData = from user in _context.Users
                               join role in _context.Roles on user.RoleId equals role.Id
                               join countries in _context.Countries on user.CountryId equals countries.Id
                               where user.RoleId == roleId && user.Id == id
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
                                   CountryId = user.CountryId,
                                   CountryCode = countries.PhoneCode,
                                   CountryFlag = countries.EmojiU
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


        public async Task<(string, bool)> GetPasswordByUserId(long id)
        {
            var user = await _context.Users
                .Where(a => a.Id == id)
                .Select(a => new { a.PasswordHash, a.IsActive })
                .FirstOrDefaultAsync();

            return (user.PasswordHash, user.IsActive.Value);
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
        public async Task<(string, Users?)> UpdateUserAsync(Users user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                var updatedUser = await _context.Users.FindAsync(user.Id);
                return (Constants.SUCCESS, updatedUser);
            }
            catch (Exception ex)
            {
                // Handle the exception as needed
                throw;
            }
        }

        public async Task<PagedResponse<UserResponse>> GetAllUsersAsync(int pageIndex, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, long? roleId, string parentRoleId)
        {


            var query = from user in _context.Users
                        join role in _context.Roles on user.RoleId equals role.Id
                        join authtype in _context.AuthenticationType on user.AuthenticationType equals authtype.Id
                        join countries in _context.Countries on user.CountryId equals countries.Id
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
                            IsActive = user.IsActive,
                            CountryId = user.CountryId,
                            CountryCode = countries.PhoneCode,
                            CountryFlag = countries.EmojiU
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