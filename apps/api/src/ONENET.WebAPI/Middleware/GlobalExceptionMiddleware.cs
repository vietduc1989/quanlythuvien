// QUAN-20260601-1634
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ONENET.Application.Common.Exceptions;
using ONENET.WebAPI.Common;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ONENET.WebAPI.Middleware
{
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
            var response = new ApiResponse(false);

            switch (exception)
            {
                case ValidationException validationException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Validation failed";
                    response.Errors = validationException.Errors.SelectMany(x => x.Value.Select(v => new ApiError { Field = x.Key, Message = v })).ToList();
                    _logger.LogWarning(validationException, "Validation error occurred: {Message}", validationException.Message);
                    break;
                case NotFoundException notFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = notFoundException.Message;
                    _logger.LogWarning(notFoundException, "Resource not found: {Message}", notFoundException.Message);
                    break;
                case ForbiddenException forbiddenException:
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    response.Message = forbiddenException.Message;
                    _logger.LogWarning(forbiddenException, "Forbidden access: {Message}", forbiddenException.Message);
                    break;
                case UnauthorizedAccessException unauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = unauthorizedAccessException.Message;
                    _logger.LogWarning(unauthorizedAccessException, "Unauthorized access: {Message}", unauthorizedAccessException.Message);
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "An unexpected error occurred.";
                    _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
                    break;
            }

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}