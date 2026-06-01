// QUAN-20260601-1634
using System.Collections.Generic;

namespace ONENET.WebAPI.Common
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public object? Data { get; set; }
        public string? Message { get; set; }
        public List<ApiError>? Errors { get; set; }

        public ApiResponse(bool success, object? data = null, string? message = null, List<ApiError>? errors = null)
        {
            Success = success;
            Data = data;
            Message = message;
            Errors = errors;
        }

        public static ApiResponse Success(object? data = null, string? message = null)
        {
            return new ApiResponse(true, data, message);
        }

        public static ApiResponse Error(string message, List<ApiError>? errors = null)
        {
            return new ApiResponse(false, null, message, errors);
        }
    }

    public class ApiError
    {
        public string? Field { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}