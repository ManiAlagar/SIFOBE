using SIFO.APIService.User.Repository.Contracts;
using SIFO.APIService.User.Service.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Request;
using SIFO.Model.Response;
using System.Text.RegularExpressions;

namespace SIFO.APIService.User.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<string>> CreateUserAsync(UserRequest users)
        {
            var errors = new List<string>();

            // Validate LastName
            if (string.IsNullOrEmpty(users.LastName))
            {
                errors.Add($"LastName {Constants.REQUIRED}");
            }

            // Validate FirstName
            if (string.IsNullOrEmpty(users.FirstName))
            {
                errors.Add($"FirstName {Constants.REQUIRED}");
            }

            // Validate Email
            if (string.IsNullOrEmpty(users.Email))
            {
                errors.Add($"Email {Constants.REQUIRED}");
            }
            else if (!Regex.IsMatch(users.Email, Constants.EMAIL_REGEX))
            {
                errors.Add(Constants.INVALID_EMAIL_FORMAT);
            }

            // Validate PhoneNumber
            if (string.IsNullOrEmpty(users.PhoneNumber))
            
                errors.Add($"PhoneNumber {Constants.REQUIRED}");
            

            // Validate PasswordHash
            if (string.IsNullOrEmpty(users.PasswordHash))
            
                errors.Add($"Password {Constants.REQUIRED}");
            
            // Return all validation errors at once
            if (errors.Any())
            {
                return ApiResponse<string>.BadRequest(string.Join(", ", errors));
            }

            // Check if email or phone number already exists
            var checkResult = await _userRepository.CheckIfEmailOrPhoneExists(users.Email, users.PhoneNumber);
            if (checkResult != null)
            {
                return checkResult;
            }

            bool isUserCreated = await _userRepository.SaveUserAsync(users);
            if (isUserCreated)
            {
                return ApiResponse<string>.Created("User Created Successfully!");
            }
            else
            {
                return ApiResponse<string>.InternalServerError("Something went wrong while creating the user.");
            }
        }

       public async Task<ApiResponse<PagedResponse<GetUserResponse>>> GetAllUsersAsync(int pageIndex, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll)
        {

        try
        {
            var result = await _userRepository.GetAllUsersAsync(pageIndex, pageSize, filter, sortColumn, sortDirection, isAll);

            if (result.StatusCode == 200)
            {
                return ApiResponse<PagedResponse<GetUserResponse>>.Success("Users fetched successfully",result.Data);
            }
            else
            {
                return ApiResponse<PagedResponse<GetUserResponse>>.InternalServerError("Failed to fetch users");
            }
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedResponse<GetUserResponse>>.InternalServerError($"An error occurred: {ex.Message}");
        }
    }

       public async Task<ApiResponse<string>> DeleteUserById(long id)
        {
           var result = await _userRepository.DeleteUserById(id);
            return result;
        }

        public async Task<ApiResponse<string>> UpdateUserAsync(UserRequest user)
        {
            var errors = new List<string>();
            if (string.IsNullOrEmpty(user.LastName))
            {
                errors.Add($"LastName {Constants.REQUIRED}");
            }

            // Validate FirstName
            if (string.IsNullOrEmpty(user.FirstName))
            {
                errors.Add($"FirstName {Constants.REQUIRED}");
            }

            // Validate Email
            if (string.IsNullOrEmpty(user.Email))
            {
                errors.Add($"Email {Constants.REQUIRED}");
            }
            else if (!Regex.IsMatch(user.Email, Constants.EMAIL_REGEX))
            {
                errors.Add(Constants.INVALID_EMAIL_FORMAT);
            }

            // Validate PhoneNumber
            if (string.IsNullOrEmpty(user.PhoneNumber))
            {
                errors.Add($"PhoneNumber {Constants.REQUIRED}");
            }

            // Return all validation errors at once
            if (errors.Any())
            {
                return ApiResponse<string>.BadRequest(string.Join(", ", errors));
            }

            var validUser=await _userRepository.GetUserById(user.Id);

            if (validUser == null)
            {
                return ApiResponse<string>.BadRequest("User doesn't exists!");
            }
            
            var result=  await _userRepository.UpdateUserAsync(user);

            return result;
        }
        public async Task<ApiResponse<GetUserResponse>> GetUserById(long? id)
        {
            
              var user =  await _userRepository.GetUserById(id);
               
            
             if (user == null)
            {
                return ApiResponse<GetUserResponse>.NotFound("User not found");
            }
            else
            {
                var userModel = new GetUserResponse
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

                return ApiResponse<GetUserResponse>.Success("Users", userModel);
            }

        }
    }
}
