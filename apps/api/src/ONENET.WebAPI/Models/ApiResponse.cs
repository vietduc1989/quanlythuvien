using System.Collections.Generic;

namespace ONENET.WebAPI.Models;

public class ApiResponse
{
    public bool Success { get; set; }
    public object? Data { get; set; }
    public string? Message { get; set; }
    public List<ApiError> Errors { get; set; } = new();

    public static ApiResponse SuccessResult(object? data = null, string? message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse FailureResult(string message, List<ApiError>? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors ?? new()
        };
    }
}

public class ApiError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public ApiError(string field, string message)
    {
        Field = field;
        Message = message;
    }
}
