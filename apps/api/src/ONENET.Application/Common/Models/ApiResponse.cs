// QUAN-20260601-105011
using ONENET.Application.Common.Exceptions;

namespace ONENET.Application.Common.Models;

/// <summary>
/// Định dạng phản hồi chuẩn cho API.
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<ValidationError> Errors { get; set; } = new List<ValidationError>();

    private ApiResponse(bool success, T? data, string? message, List<ValidationError>? errors = null)
    {
        Success = success;
        Data = data;
        Message = message;
        Errors = errors ?? new List<ValidationError>();
    }

    /// <summary>
    /// Tạo phản hồi thành công.
    /// </summary>
    public static ApiResponse<T> Succeeded(T data, string? message = null)
    {
        return new ApiResponse<T>(true, data, message);
    }

    /// <summary>
    /// Tạo phản hồi lỗi.
    /// </summary>
    public static ApiResponse<T> Failed(string message, List<ValidationError>? errors = null)
    {
        return new ApiResponse<T>(false, default, message, errors);
    }
}

/// <summary>
/// Phiên bản không generic của ApiResponse cho các trường hợp không có dữ liệu trả về.
/// </summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<ValidationError> Errors { get; set; } = new List<ValidationError>();

    private ApiResponse(bool success, string? message, List<ValidationError>? errors = null)
    {
        Success = success;
        Message = message;
        Errors = errors ?? new List<ValidationError>();
    }

    public static ApiResponse Succeeded(string? message = null)
    {
        return new ApiResponse(true, message);
    }

    public static ApiResponse Failed(string message, List<ValidationError>? errors = null)
    {
        return new ApiResponse(false, message, errors);
    }
}