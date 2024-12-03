using Microsoft.AspNetCore.Http;

namespace SIFO.Model.Response
{
    public class ApiResponse<T>
    {
        public ApiResponse(int statusCode, string? message = default, T? data = default, bool isSuccess = false)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
            IsSuccess = isSuccess;
        }

        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public static ApiResponse<T> Success(string? message = "Success", T? data = default)
        {
            return new ApiResponse<T>(StatusCodes.Status200OK, message, data, true);
        }

        public static ApiResponse<T> Created(string? message = "Successfully Created", T? data = default)
        {
            return new ApiResponse<T>(StatusCodes.Status201Created, message, data, true);
        }

        public static ApiResponse<T> NotFound(string? message = "Not Found", T? data = default)
        {
            return new ApiResponse<T>(StatusCodes.Status404NotFound, message, data, false);
        }
        public static ApiResponse<T> NoContent(string? message = "No Content", T? data = default)
        {
            return new ApiResponse<T>(StatusCodes.Status204NoContent, message, data, false);
        }


        public static ApiResponse<T> BadRequest(string? message = "Bad Request", T? data = default)
        {
            return new ApiResponse<T>(StatusCodes.Status400BadRequest, message, data, false);
        }

        public static ApiResponse<T> InternalServerError(string? message = "Something Went Wrong")
        {
            return new ApiResponse<T>(StatusCodes.Status500InternalServerError, message, default, false);
        }
        public static ApiResponse<T> UnAuthorized(string? message = "UnAuthorized")
        {
            return new ApiResponse<T>(StatusCodes.Status401Unauthorized, message, default, false);
        }

        public static ApiResponse<T> Conflict(string? message = "Conflict")
        {
            return new ApiResponse<T>(StatusCodes.Status409Conflict, message, default, false);
        }

        public static ApiResponse<T> Forbidden(string? message = "Forbidden")
        {
            return new ApiResponse<T>(StatusCodes.Status403Forbidden, message, default, false);
        }
    }
}
