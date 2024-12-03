using Microsoft.EntityFrameworkCore;
using SIFO.APIService.User.Repository.Contracts;
using SIFO.Common.Contracts;
using SIFO.Model.Entity;
using SIFO.Model.Request;
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
        public async Task<bool> SaveUserAsync(UserRequest user)
        {
            using (var context = await _context.Database.BeginTransactionAsync())
                try
                {
                    var encryptedPassword = await _commonService.EncryptPassword(user.PasswordHash);
                    var userData = new Users()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        PasswordHash = encryptedPassword,
                        PhoneNumber = user.PhoneNumber,
                        RoleId= 2,
                        FiscalCode = user.FiscalCode,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = 1,
                        IsTempPassword = true
                    };
                    var addedData = await _context.Users.AddAsync(userData);
                    _context.SaveChanges();
                    await _context.Database.CommitTransactionAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await _context.Database.RollbackTransactionAsync();
                    return false;

                }
        }
        public async Task<ApiResponse<string>> CheckIfEmailOrPhoneExists(string email, string phoneNumber, long? userId)
        {
            if (await _context.Users.AnyAsync(u => u.Email == email && u.Id != userId && u.IsActive == true))
            {
                return ApiResponse<string>.Conflict("Email already exists.");
            }
            if (await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber && u.Id != userId && u.IsActive == true))
            {
                return ApiResponse<string>.Conflict("Phone number already exists.");
            }
            return null;
        }

        public IQueryable<Users> GetUsersQueryable()
        {
            return _context.Users.AsQueryable();
        }
        public async Task<ApiResponse<string>> DeleteUserById(long id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChangesAsync();
                return ApiResponse<string>.Success("User Deleted Successfully");
            }
            else
            {
                return ApiResponse<string>.InternalServerError("User doesn't exist");
            }
        }

         public async Task<Users> GetUserById(long? id)
        {
            return await _context.Users.FindAsync(id);
            
        }
        public async Task<ApiResponse<string>> UpdateUserAsync(UserRequest user)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                var users = await GetUserById(user.Id);
                try
                {
                    var checkResult = await CheckIfEmailOrPhoneExists(user.Email, user.PhoneNumber, user.Id);
                    if (checkResult != null)
                    {
                        return checkResult;
                    }
                    users.LastName = user.LastName;
                    users.FirstName = user.FirstName;
                    users.Email = user.Email;
                    users.FiscalCode = user.FiscalCode;
                    users.PhoneNumber = user.PhoneNumber;
                    users.UpdatedDate = DateTime.UtcNow;
                    users.UpdatedBy = 1;

                    _context.Users.Update(users);
                    _context.SaveChangesAsync();

                    _context.Database.CommitTransaction();

                    return ApiResponse<string>.Success("User Updated Successfully !");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(); return ApiResponse<string>.InternalServerError($"Error while updating the user: {ex.Message}");
                }

            }
        }
        public async Task<ApiResponse<PagedResponse<UserResponse>>> GetAllUsersAsync(int pageIndex, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll)
        {

            var query = _context.Users.Select(user => new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ZipCode = user.ZipCode,
                FiscalCode = user.FiscalCode,
                CreatedDate = user.CreatedDate,
                CreatedBy = user.CreatedBy,
                UpdatedDate = user.UpdatedDate,
                UpdatedBy = user.UpdatedBy,
                IsActive = user.IsActive
            });

            var count = query.Count();
            PagedResponse<UserResponse> pagedResponse = new PagedResponse<UserResponse>();

            if (isAll)
            {
                var result = query.ToList();
                
                pagedResponse.Result = result.AsEnumerable();
                pagedResponse.TotalCount = count;
                pagedResponse.TotalPages = 1;
                pagedResponse.CurrentPage = 1;
                return ApiResponse<PagedResponse<UserResponse>>.Success("Data", pagedResponse);
            }

            string orderByExpression = $"{sortColumn} {sortDirection}";

            // Apply filtering
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

            return ApiResponse<PagedResponse<UserResponse>>.Success("Data", pagedResponse);
        }

        public async Task<Role> GetRoleById(long? id)
        {
            try
            { 
                var result = await _context.Roles.Where(a=>a.Id == id).SingleOrDefaultAsync();
                return result;
            } 
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<UserResponse>> GetUserByRoleId(long? roleId)
        {
            try
            {
                var users = await _context.Users
                .Where(a => a.RoleId == roleId)
                .Select(user => new UserResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    ZipCode = user.ZipCode,
                    FiscalCode = user.FiscalCode,
                    CreatedDate = user.CreatedDate,
                    CreatedBy = user.CreatedBy,
                    UpdatedDate = user.UpdatedDate,
                    UpdatedBy = user.UpdatedBy,
                    IsActive = user.IsActive
                })
                .ToListAsync();
                return users;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}