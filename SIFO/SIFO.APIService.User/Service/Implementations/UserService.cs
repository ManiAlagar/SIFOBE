using AutoMapper;
using SIFO.APIService.Master.Repository.Contracts;
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

        public async Task<ApiResponse<string>> CreateUserAsync(UserRequest request)
        {
            var tokenData = await _commonService.GetDataFromToken();
            if (!tokenData.ParentRoleId.Contains(request.RoleId.ToString()))
                return ApiResponse<string>.Forbidden(Constants.INVALID_ROLE);

            if (!string.IsNullOrEmpty(request.ProfileImg))
            {
                var writtenPath = await _commonService.SaveFileAsync(request.ProfileImg, null, Path.Join(_configuration["FileUploadPath:Path"], $"Users/{request.UserId}"));
                if (writtenPath is null)
                    return ApiResponse<string>.BadRequest("ProfileImg is invalid");
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
            mappedResult.PharmacyIds = request.PharmacyIds;
            mappedResult.HospitalIds = request.HospitalIds;
            mappedResult.PasswordHash = await _commonService.HashPassword(request.PasswordHash);
            
            var message = await _userRepository.CreateUserAsync(mappedResult, tokenData.UserId);
            //if (request.PharmacyIds != null && request.PharmacyIds.Any())
            //{
            //    foreach (var items in request.PharmacyIds)
            //    {
            //        _userRepository.InsertUserPharmacyMapping(userId, items);
            //    }
            //}
            if (message == Constants.SUCCESS)
            {
              
                    var filePath = _configuration["Templates:WelcomeEmail"];
                string subject = $"Welcome User";
                string body = File.ReadAllText(filePath)
              .Replace("[UserName]", $"{request.FirstName} {request.LastName}")
              .Replace("[UserEmail]", $"{request.Email}")
              .Replace("[UserPassword]", request.PasswordHash);

     


                //var mailResponse = await _sendGridService.SendMailAsync(request.Email, subject, body, $"{userData.FirstName} {userData.LastName}");  
                var toUser = new string[] { request.Email };
                var mailResponse = await _commonService.SendMail(toUser.ToList(), null, subject, body);
                if (!mailResponse)
                    //if (!mailResponse.IsSuccess)
                    return ApiResponse<string>.InternalServerError("something went wrong while sending the mail");
                return ApiResponse<string>.Created(Constants.SUCCESS);
           
            }
            return ApiResponse<string>.InternalServerError(message);
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
            var (passwordData,isActive) = await _userRepository.GetPasswordByUserId(request.UserId.Value);
            if (passwordData == null)
                return ApiResponse<string>.NotFound(Constants.NOT_FOUND);

            if (!tokenData.ParentRoleId.Contains(request.RoleId.ToString()) && tokenData.RoleId != request.RoleId)
                return ApiResponse<string>.Forbidden(Constants.INVALID_ROLE);
            if (!string.IsNullOrEmpty(request.ProfileImg))
            {
                var writtenPath = await _commonService.SaveFileAsync(request.ProfileImg, null, Path.Join(_configuration["FileUploadPath:Path"], $"Users/{request.UserId}"));
                if (writtenPath is null)
                    return ApiResponse<string>.BadRequest("ProfileImg is invalid");
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
            var (status, user) =  await _userRepository.UpdateUserAsync(mappedResult, tokenData.UserId);
            if (status == Constants.SUCCESS)
            {
                if(isActive != request.IsActive)
                {
                    string statusMessage = user.IsActive.Value ? "activated" : "deactivated";
                    string emailSubject = user.IsActive.Value ? "Account Activation" : "Account Deactivation";
                    var filePath = _configuration["Templates:StatusTemplate"];
                    string subject = emailSubject;
                    string body = File.ReadAllText(filePath)
                  .Replace("[UserName]", $"{user.FirstName} {user.LastName}")
                  .Replace("[Status]", $"{statusMessage}")
                  .Replace("[status]", statusMessage.ToLower());
                    //var mailResponse = await _sendGridService.SendMailAsync(request.Email, subject, body, $"{userData.FirstName} {userData.LastName}");  
                    var toUser = new string[] { request.Email };
                    var mailResponse = await _commonService.SendMail(toUser.ToList(), null, subject, body);
                    if (!mailResponse)
                        //if (!mailResponse.IsSuccess)
                        return ApiResponse<string>.InternalServerError("something went wrong while sending the mail");
                }
                return ApiResponse<string>.Success(Constants.SUCCESS);
            }
          

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
                else 
                    return ApiResponse<UserResponse>.Success(Constants.SUCCESS, user);
            }
            else
                return ApiResponse<UserResponse>.Forbidden();
        }
    }
}
