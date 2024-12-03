using SIFO.APIService.User.Repository.Contracts;
using SIFO.APIService.User.Service.Contracts;
using SIFO.Common.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.User.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository; 
        private readonly ICommonService _commonService; 
        private readonly IConfiguration _configuration; 

        public UserService(IUserRepository userRepository, ICommonService commonService , IConfiguration configuration)
        {
            _userRepository = userRepository; 
            _commonService = commonService;  
            _configuration = configuration;
        }

        public async Task<ApiResponse<string>> CreateUserAsync(UserRequest users)
        {
            var checkResult = await _userRepository.CheckIfEmailOrPhoneExists(users.Email, users.PhoneNumber);
            if (checkResult != null)
                return checkResult;

            bool isUserCreated = await _userRepository.SaveUserAsync(users);
            if (isUserCreated)
                return ApiResponse<string>.Created(Constants.SUCCESS);
            else
                return ApiResponse<string>.InternalServerError("Something went wrong while creating the user.");
        }

       public async Task<ApiResponse<PagedResponse<UserResponse>>> GetAllUsersAsync(int pageIndex, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll)
        {

        try
        {
            var result = await _userRepository.GetAllUsersAsync(pageIndex, pageSize, filter, sortColumn, sortDirection, isAll);

            if (result.StatusCode == 200)
            {
                return ApiResponse<PagedResponse<UserResponse>>.Success("Users fetched successfully",result.Data);
            }
            else
                return ApiResponse<PagedResponse<UserResponse>>.InternalServerError("Failed to fetch users");
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedResponse<UserResponse>>.InternalServerError($"An error occurred: {ex.Message}");
        }
    }

       public async Task<ApiResponse<string>> DeleteUserById(long id)
        {
           var result = await _userRepository.DeleteUserById(id);
            return result;
        }

        public async Task<ApiResponse<string>> UpdateUserAsync(UserRequest request)
        { 
            var result=  await _userRepository.UpdateUserAsync(request);
            return result;
        }
        public async Task<ApiResponse<UserResponse>> GetUserById(long? id)
        {
            var user =  await _userRepository.GetUserById(id);
            
            if (user == null)
            {
            return ApiResponse<UserResponse>.NotFound("User not found");
            }
            else
            {
                var userModel = new UserResponse
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
                };

                return ApiResponse<UserResponse>.Success(Constants.SUCCESS, userModel);
            }
        }

        public async Task<ApiResponse<List<UserResponse>>> GetUserByRoleId(long? roleId)
        {
            var tokenData = await _commonService.GetDataFromToken();
            
            if (roleId == null)
                roleId = tokenData.RoleId; 

            var role = await _userRepository.GetRoleById(roleId);
            if (role is null)
                return ApiResponse<List<UserResponse>>.NotFound("role not found");

            var users = await _userRepository.GetUserByRoleId(roleId);


            return ApiResponse<List<UserResponse>>.Success(Constants.SUCCESS,users);

        }
    }
}
