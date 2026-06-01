// QUAN-20260601-105011
using System.Net;
using System.Text.Json;
using ONENET.Application.Common.Exceptions;
using ONENET.Application.Common.Models;
using Serilog;

namespace ONENET.WebAPI.Middleware;

/// <summary>
/// Middleware toàn cục để bắt và xử lý các ngoại lệ, trả về phản hồi API chuẩn.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = new ApiResponse();
        var statusCode = HttpStatusCode.InternalServerError;

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                response.Message = "One or more validation errors occurred.";
                response.Errors = validationException.Errors.SelectMany(kvp => kvp.Value.Select(msg => new ValidationError { Field = kvp.Key, Message = msg })).ToList();
                _logger.LogWarning(validationException, "Validation error occurred: {Message}", validationException.Message);
                break;
            case NotFoundException _:
                statusCode = HttpStatusCode.NotFound;
                response.Message = exception.Message;
                _logger.LogWarning(exception, "Not Found error occurred: {Message}", exception.Message);
                break;
            case UnauthorizedAccessException _: // Example for 401, though JWT middleware handles this mostly
                statusCode = HttpStatusCode.Unauthorized;
                response.Message = "Unauthorized access.";
                _logger.LogWarning(exception, "Unauthorized access: {Message}", exception.Message);
                break;
            case ForbiddenException _: // Custom exception if needed for 403
                statusCode = HttpStatusCode.Forbidden;
                response.Message = "Forbidden. User does not have sufficient permissions.";
                _logger.LogWarning(exception, "Forbidden access: {Message}", exception.Message);
                break;
            case InvalidOperationException invOpEx when invOpEx.Message.Contains("Concurrency conflict"): // Explicitly check for concurrency message
                statusCode = HttpStatusCode.Conflict;
                response.Message = invOpEx.Message;
                _logger.LogWarning(invOpEx, "Concurrency conflict error: {Message}", invOpEx.Message);
                break;
            default:
                statusCode = HttpStatusCode.InternalServerError;
                response.Message = "An unexpected error occurred. Please try again later.";
                _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
                break;
        }

        response.Success = false;
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }

    // A placeholder for a potential ForbiddenException if not using default AuthZ handlers
    private class ForbiddenException : Exception
    {
        public ForbiddenException() : base() { }
        public ForbiddenException(string message) : base(message) { }
        public ForbiddenException(string message, Exception innerException) : base(message, innerException) { }
    }
}