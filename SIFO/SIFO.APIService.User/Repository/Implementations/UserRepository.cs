using SIFO.Common.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Entity;
using SIFO.Model.Response;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using SIFO.APIService.User.Repository.Contracts;


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

        public async Task<string> CreateUserAsync(Users user)
        {
            using (var context = await _context.Database.BeginTransactionAsync()) 
            {
                try
                {
                    var tokenData = _commonService.GetDataFromToken();
                    var encryptedPassword = await _commonService.EncryptPassword(user.PasswordHash);
                    var userData = new Users()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        PasswordHash = encryptedPassword,
                        PhoneNumber = user.PhoneNumber,
                        RoleId = user.RoleId,
                        FiscalCode = user.FiscalCode,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = tokenData.Id,
                        IsTempPassword = true
                    };
                    var result = await _context.Users.AddAsync(userData);
                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();
                    return Constants.SUCCESS;
                }
                catch (Exception ex)
                {
                    await _context.Database.RollbackTransactionAsync();
                    throw;
                } 
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

        public IQueryable<Users> GetUsersQueryable()
        {
            try
            {
                return _context.Users.AsQueryable();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> DeleteUserById(long id)
        {
            using (var context = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var otpDelete = await _context.OtpRequests.Where(x => x.UserId == id).ToListAsync();
                    _context.OtpRequests.RemoveRange(otpDelete);

                    var entity = await _context.Users.Where(x => x.Id == id).SingleOrDefaultAsync();
                    if (entity != null)
                    {
                        _context.Users.Remove(entity);
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

         public async Task<Users> GetUserById(long? id)
        {
            return await _context.Users.AsNoTracking().Where(a=>a.Id == id).SingleOrDefaultAsync();
        }

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

        public async Task<PagedResponse<UserResponse>> GetAllUsersAsync(int pageIndex, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll)
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
                IsActive = user.IsActive
            });

            var count = query.Count();
            PagedResponse<UserResponse> pagedResponse = new PagedResponse<UserResponse>();

            if (isAll)
            {
                var result = query.Where(a=>a.IsActive == true).ToList();
                
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