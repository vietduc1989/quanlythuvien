using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ONENET.Application.Common.Exceptions;
using ONENET.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ONENET.WebAPI.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (FluentValidation.ValidationException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var errors = ex.Errors
                .Select(e => new ApiError(e.PropertyName, e.ErrorMessage))
                .ToList();

            var response = ApiResponse.FailureResult("Dữ liệu đầu vào không hợp lệ.", errors);
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (ONENET.Application.Common.Exceptions.ValidationException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var errors = ex.Errors
                .SelectMany(kv => kv.Value.Select(message => new ApiError(kv.Key, message)))
                .ToList();

            var response = ApiResponse.FailureResult("Dữ liệu đầu vào không hợp lệ.", errors);
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (NotFoundException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status404NotFound;

            var response = ApiResponse.FailureResult(ex.Message);
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (ForbiddenException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status403Forbidden;

            var response = ApiResponse.FailureResult(ex.Message);
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            var response = ApiResponse.FailureResult(ex.Message);
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Đã xảy ra lỗi hệ thống không mong muốn.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = ApiResponse.FailureResult($"Lỗi hệ thống: {ex.Message}");
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}

