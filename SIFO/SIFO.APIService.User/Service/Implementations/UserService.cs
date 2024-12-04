using AutoMapper;
using SIFO.APIService.User.Repository.Contracts;
using SIFO.APIService.User.Service.Contracts;
using SIFO.Common.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;
using SIFO.Utility.Implementations;

namespace SIFO.APIService.User.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository; 
        private readonly ICommonService _commonService; 
        private readonly IConfiguration _configuration; 
        private readonly IMapper _mapper; 

        public UserService(IUserRepository userRepository, ICommonService commonService , IConfiguration configuration, IMapper mapper)
        {
            _userRepository = userRepository; 
            _commonService = commonService;  
            _configuration = configuration; 
            _mapper = mapper;
        }

        public async Task<ApiResponse<Users>> CreateUserAsync(UserRequest request)
        {
            var checkResult = await _userRepository.CheckIfEmailOrPhoneExists(request.Email, request.PhoneNumber);
            if (checkResult != Constants.SUCCESS)
                return ApiResponse<Users>.Conflict(checkResult);
            var mappedResult  = _mapper.Map<Users>(request);
            var userData = await _userRepository.CreateUserAsync(mappedResult);
            if (userData is not null)
                return ApiResponse<Users>.Created(Constants.SUCCESS, userData);
            return ApiResponse<Users>.InternalServerError("Something went wrong while creating the user.");
        }

       public async Task<ApiResponse<PagedResponse<UserResponse>>> GetAllUsersAsync(int pageIndex, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll)
        {
            try
            {
                var isValid = await HelperService.ValidateGet(pageIndex, pageSize, filter, sortColumn, sortDirection);
                if (isValid.Any())
                    return ApiResponse<PagedResponse<UserResponse>>.BadRequest(isValid[0]);
                var response = await _userRepository.GetAllUsersAsync(pageIndex, pageSize, filter, sortColumn, sortDirection, isAll);
                return ApiResponse<PagedResponse<UserResponse>>.Success(Constants.SUCCESS, response); 
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResponse<UserResponse>>.InternalServerError($"An error occurred: {ex.Message}");
            }
       }

       public async Task<ApiResponse<string>> DeleteUserById(long id)
        {
           //var result = await _userRepository.DeleteUserById(id);
           // return result;

            var response = await _userRepository.DeleteUserById(id);
            if (response == Constants.NOT_FOUND)
                return new ApiResponse<string>(StatusCodes.Status404NotFound, Constants.CITY_NOT_FOUND);
            return ApiResponse<string>.Success(Constants.SUCCESS, response);
        }

        public async Task<ApiResponse<Users>> UpdateUserAsync(UserRequest request)
        {
            var tokenData =await _commonService.GetDataFromToken();
            var users = await GetUserById(request.UserId);
            if (users is null)
                return ApiResponse<Users>.NotFound();
            var checkResult = await _userRepository.CheckIfEmailOrPhoneExists(request.Email, request.PhoneNumber, request.UserId);
            
            if (checkResult != null)
                return ApiResponse<Users>.Conflict(checkResult);
            var mappedResult = _mapper.Map<Users>(request); 
            mappedResult.UpdatedBy = Convert.ToInt64(tokenData.UserId);
            var result=  await _userRepository.UpdateUserAsync(mappedResult);
            return ApiResponse<Users>.Success(Constants.SUCCESS, result);
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
