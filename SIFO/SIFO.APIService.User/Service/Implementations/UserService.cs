using AutoMapper;
using SIFO.APIService.User.Repository.Contracts;
using SIFO.APIService.User.Service.Contracts;
using SIFO.Common.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;
using SIFO.Utility.Implementations;
using System.Diagnostics.Eventing.Reader;

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

        public async Task<ApiResponse<string>> CreateUserAsync(UserRequest request)
        {
            var tokenData = await _commonService.GetDataFromToken();
          
            if (!tokenData.ParentRoleId.Contains(request.RoleId.ToString()))
                return ApiResponse<string>.Forbidden(Constants.INVALID_ROLE);

            if (!string.IsNullOrEmpty(request.ProfileImg))
            {
                var writtenPath = await _commonService.SaveFileAsync(request.ProfileImg, null, Path.Join(_configuration["FileUploadPath:Path"], $"Users/{request.UserId}"));
                if (writtenPath is null)
                    return ApiResponse<string>.InternalServerError();
                else
                    request.ProfileImg = writtenPath;
            }

            var checkResult = await _userRepository.CheckIfEmailOrPhoneExists(request.Email, request.PhoneNumber);
            if (checkResult != Constants.SUCCESS)
                return ApiResponse<string>.Conflict(checkResult);

            var mappedResult = _mapper.Map<Users>(request);
            mappedResult.CreatedBy = Convert.ToInt64(tokenData.UserId);
            mappedResult.CreatedDate = DateTime.UtcNow;
            mappedResult.IsTempPassword = true;
            mappedResult.AuthenticationType = request.AuthenticationType;

            var userData = await _userRepository.CreateUserAsync(mappedResult);

            if (userData == Constants.SUCCESS)
                return ApiResponse<string>.Created(Constants.SUCCESS);

            return ApiResponse<string>.InternalServerError("Something went wrong while creating the user.");
        }
        public async Task<ApiResponse<PagedResponse<UserResponse>>> GetAllUsersAsync(int pageIndex, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, long? roleId)
        {
            var isValid = await HelperService.ValidateGet(pageIndex, pageSize, filter, sortColumn, sortDirection);

            if (isValid.Any())
                return ApiResponse<PagedResponse<UserResponse>>.BadRequest(isValid[0]);
            var tokenData = await _commonService.GetDataFromToken();

            
            if (tokenData.ParentRoleId.Contains(roleId.ToString()))
            {
                var response = await _userRepository.GetAllUsersAsync(pageIndex, pageSize, filter, sortColumn, sortDirection, isAll, roleId, tokenData.ParentRoleId);
                return ApiResponse<PagedResponse<UserResponse>>.Success(Constants.SUCCESS, response);
            }
           
            return ApiResponse<PagedResponse<UserResponse>>.Forbidden();

        }

        public async Task<ApiResponse<string>> DeleteUserById(long id, long roleId)
        {
            var tokenData = await _commonService.GetDataFromToken();
            if (tokenData.ParentRoleId.Contains(roleId.ToString()))
            {
                var response = await _userRepository.DeleteUserById(id, roleId, tokenData.ParentRoleId);
                if (response == Constants.NOT_FOUND)
                    return ApiResponse<string>.NotFound();
                else
                {
                    return ApiResponse<string>.Success(Constants.SUCCESS);
                }
            }
           

            return ApiResponse<string>.Forbidden();
        }

        //public async Task<ApiResponse<string>> UpdateUserAsync(UserRequest request)
        //{
        //    var tokenData = await _commonService.GetDataFromToken();
        //    var users = await _userRepository.GetUserById(request.UserId);
        //    if (users == null)
        //        return ApiResponse<string>.Forbidden("You are not allowed to edit this user");
        //    var checkResult = await _userRepository.CheckIfEmailOrPhoneExists(request.Email, request.PhoneNumber, request.UserId);

        //    if (checkResult != Constants.SUCCESS)
        //        return ApiResponse<string>.Conflict(checkResult);

        //    var mappedResult = _mapper.Map<Users>(request);
        //    mappedResult.Id = request.UserId.Value;
        //    mappedResult.UpdatedBy = Convert.ToInt64(tokenData.UserId);

        //    var result = await _userRepository.UpdateUserAsync(mappedResult, users.RoleId, tokenData.ParentRoleId);
        //    if (result == Constants.SUCCESS)
        //    {
        //        return ApiResponse<string>.Success(Constants.SUCCESS);
        //    }
        //    else if (result == Constants.INVALID_ROLE)
        //    {
        //        return ApiResponse<string>.BadRequest(Constants.INVALID_ROLE);
        //    }
        //    else
        //    {
        //        //nothing 
        //    }
        //    return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        //}
        public async Task<ApiResponse<string>> UpdateUserAsync(UserRequest request)
        {
            var tokenData = await _commonService.GetDataFromToken();
            var passwordData = await _userRepository.GetPasswordByUserId(request.UserId.Value);
            if (passwordData == null)
                return ApiResponse<string>.NotFound(Constants.NOT_FOUND);

            if (!tokenData.ParentRoleId.Contains(request.RoleId.ToString()))
                return ApiResponse<string>.Forbidden(Constants.INVALID_ROLE);
            if (!string.IsNullOrEmpty(request.ProfileImg))
            {
                var writtenPath = await _commonService.SaveFileAsync(request.ProfileImg, null, Path.Join(_configuration["FileUploadPath:Path"], $"Users/{request.UserId}"));
                if (writtenPath is null)
                    return ApiResponse<string>.InternalServerError();
                else
                    request.ProfileImg = writtenPath;
            }
            var checkResult = await _userRepository.CheckIfEmailOrPhoneExists(request.Email, request.PhoneNumber, request.UserId);

            if (checkResult != Constants.SUCCESS)
                return ApiResponse<string>.Conflict(checkResult);

            var mappedResult = _mapper.Map<Users>(request);
            mappedResult.Id = request.UserId.Value;
            mappedResult.UpdatedBy = Convert.ToInt64(tokenData.UserId);
            mappedResult.UpdatedDate = DateTime.UtcNow;
            mappedResult.PasswordHash = passwordData;

            var result = await _userRepository.UpdateUserAsync(mappedResult);
            if (result == Constants.SUCCESS)
                return ApiResponse<string>.Success(Constants.SUCCESS);

            return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<UserResponse>> GetUserById(long? id, long RoleId)
        {
            var tokenData = await _commonService.GetDataFromToken();


            if (tokenData.ParentRoleId.Contains(RoleId.ToString()) || tokenData.RoleId == RoleId)
            {
                var user = await _userRepository.GetUserById(id, RoleId, tokenData.ParentRoleId);
                if (user == null)
                    return ApiResponse<UserResponse>.NotFound();
                else return ApiResponse<UserResponse>.Success(Constants.SUCCESS, user);
            }

            else
            {
                return ApiResponse<UserResponse>.Forbidden();
            }
        }
    }
}
