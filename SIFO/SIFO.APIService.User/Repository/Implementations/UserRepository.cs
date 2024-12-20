using Microsoft.EntityFrameworkCore;
using SIFO.APIService.Hospital.Repository.Contracts;
using SIFO.APIService.User.Repository.Contracts;
using SIFO.Common.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Entity;
using SIFO.Model.Entity.SIFO.Model.Entity;
using SIFO.Model.Response;
using System.Linq.Dynamic.Core;


namespace SIFO.APIService.User.Repository.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly SIFOContext _context;
        private readonly ICommonService _commonService;
        private readonly IPharmacyRepository _pharmacyRepository;

        public UserRepository(SIFOContext context, ICommonService commonService,IPharmacyRepository pharmacyRepository)
        {
            _context = context;
            _commonService = commonService;
            _pharmacyRepository = pharmacyRepository;
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

        public async Task<bool> CreateUserPharmacyMapping(Users user, string userId)
        {
                try
                {
                    if (user.PharmacyIds != null && user.PharmacyIds.Any())
                    {
                        var retailPharmacyTypeId = await _pharmacyRepository.GetRetailPharmacyAsync();
                        // Check if all pharmacy IDs exist
                        bool allPharmaciesExist = await _context.Pharmacies
                            .Where(p => user.PharmacyIds.Contains(p.Id) && p.IsActive && p.PharmacyTypeId != retailPharmacyTypeId && p.CreatedBy == Convert.ToInt64(userId))
                            .CountAsync() == user.PharmacyIds.Count;

                        if (!allPharmaciesExist)
                        {
                            return false;
                        }

                        var userPharmacyMappings = user.PharmacyIds.Select(pharmacyId => new UserPharmacyMapping
                        {
                            userId = user.Id,
                            PharmacyId = pharmacyId
                        }).ToList();

                        await _context.UserPharmacyMappings.AddRangeAsync(userPharmacyMappings);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
        }

        public async Task<bool> CreateUserHospitalMapping(Users user, string userId)
        {
                try
                {
                    if(user.HospitalIds != null && user.HospitalIds.Any() && !user.HospitalIds.Contains(0))
                        {
                        bool allHospitals = await _context.HospitalFacilities
                            .Where(a => user.HospitalIds.Contains(a.Id) && a.IsActive && a.CreatedBy == Convert.ToInt64(userId))
                            .CountAsync() == user.HospitalIds.Count();
                        if (!allHospitals)
                        {
                            return false;
                        }

                        var addedData = user.HospitalIds.Select(a => new UserHospitalMapping
                        {
                            UserId = user.Id,
                            HospitalId = a

                        }).ToList();
                        await _context.UserHospitalMappings.AddRangeAsync(addedData);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                        else
                    {
                        return false; 
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
        }


        public async Task<string> CreateUserAsync(Users user,string userId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var roleName = await GetRoleById(user.RoleId);
                    var result = await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    if (roleName.Name == Constants.ROLE_HOSPITAL_PHARMACY_SUPERVISOR || roleName.Name == Constants.ROLE_HOSPITAL_PHARMACY_OPERATOR)
                    {
                        user.Id = result.Entity.Id;
                        bool resultMsg = await CreateUserPharmacyMapping(user, userId);
                        if (!resultMsg)
                        {
                            await transaction.RollbackAsync();
                            return "Error while saving Pharmacy Details.";
                        }
                    }
                    if(roleName.Name == Constants.ROLE_HOSPITAL_REFERENT)
                    {
                        user.Id = result.Entity.Id;
                        bool resultMsg = await CreateUserHospitalMapping(user, userId);
                        if (!resultMsg)
                        {
                            await transaction.RollbackAsync();
                            return "Error while saving Hospital Details.";
                        }
                    }

                    await transaction.CommitAsync();
                    return Constants.SUCCESS;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    return $"An error occurred while creating the user: {ex.Message}";
                }
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
                                   CountryFlag = countries.EmojiU,
                                   Pharmacy = (from pharmacy in _context.UserPharmacyMappings
                                               join pharm in _context.Pharmacies on pharmacy.PharmacyId equals pharm.Id
                                               where pharmacy.userId == user.Id
                                               select pharm).ToList(),
                                   Hospital = (from hospital in _context.UserHospitalMappings
                                               join hosp in _context.HospitalFacilities on hospital.HospitalId equals hosp.Id
                                               where hospital.UserId == user.Id
                                               select hosp).ToList()
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
                        var deleteUserHospitalMappings = await _context.UserHospitalMappings.Where(x => x.UserId == userToDelete.Id).ToListAsync();
                        _context.UserHospitalMappings.RemoveRange(deleteUserHospitalMappings);
                        await _context.SaveChangesAsync();
                        var deleteUserPharmacyMappings = await _context.UserPharmacyMappings.Where(x => x.userId == userToDelete.Id).ToListAsync();
                        _context.UserPharmacyMappings.RemoveRange(deleteUserPharmacyMappings);
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
        public async Task<(string, Users?)> UpdateUserAsync(Users user,string userId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var roleName = await GetRoleById(user.RoleId);
                    var result =  _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    var updatedUser = await _context.Users.FindAsync(user.Id);

                    if (roleName.Name == Constants.ROLE_HOSPITAL_PHARMACY_SUPERVISOR || roleName.Name == Constants.ROLE_HOSPITAL_PHARMACY_OPERATOR)
                    {
                        var deletedData = await _context.UserPharmacyMappings.Where(x => x.userId == user.Id).ToListAsync();
                        _context.UserPharmacyMappings.RemoveRange(deletedData);
    
                        await _context.SaveChangesAsync();

                        bool resultMsg = await CreateUserPharmacyMapping(user, userId);
                        if (!resultMsg)
                        {
                            await transaction.RollbackAsync();
                            return ("Error while saving Pharmacy Details.",null);
                        }
                    }
                    if (roleName.Name == Constants.ROLE_HOSPITAL_REFERENT)
                    {
                        var deletedData = await _context.UserHospitalMappings.Where(x => x.UserId == user.Id).ToListAsync();
                        _context.UserHospitalMappings.RemoveRange(deletedData);
                
                        await _context.SaveChangesAsync();

                        bool resultMsg = await CreateUserHospitalMapping(user, userId);
                        if (!resultMsg)
                        {
                            await transaction.RollbackAsync();
                            return ("Error while saving Hospital Details.", null);
                        }
                    }

                    await transaction.CommitAsync();
                    return (Constants.SUCCESS, updatedUser);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    return ($"An error occurred while creating the user: {ex.Message}",null);
                }
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
                            CountryFlag = countries.EmojiU,
                            Pharmacy = (from pharmacy in _context.UserPharmacyMappings
                                        join pharm in _context.Pharmacies on pharmacy.PharmacyId equals pharm.Id
                                        where pharmacy.userId == user.Id
                                        select pharm).ToList(),
                            Hospital = (from hospital in _context.UserHospitalMappings
                                        join hosp in _context.HospitalFacilities on hospital.HospitalId equals hosp.Id
                                        where hospital.UserId == user.Id
                                        select hosp).ToList()
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