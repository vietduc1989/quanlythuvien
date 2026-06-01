// QUAN-20260601-105011
using MediatR;
using Microsoft.Extensions.Logging;
using ONENET.Application.Common.Interfaces;

namespace ONENET.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior để ghi log cho mỗi request được xử lý.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;
    private readonly ICurrentUser _currentUser;

    public LoggingBehavior(ILogger<TRequest> logger, ICurrentUser currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUser.UserId ?? "Anonymous";
        var userName = _currentUser.UserName ?? "Anonymous";

        _logger.LogInformation("ONENET Request: {RequestName} started by User {UserId} ({UserName})",
            requestName, userId, userName);

        TResponse response;
        try
        {
            response = await next();
            _logger.LogInformation("ONENET Request: {RequestName} completed successfully by User {UserId} ({UserName})",
                requestName, userId, userName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ONENET Request: {RequestName} failed for User {UserId} ({UserName}) with error: {ErrorMessage}",
                requestName, userId, userName, ex.Message);
            throw;
        }

        return response;
    }
}