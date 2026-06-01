// QUAN-20260601-1634
using MediatR;
using Microsoft.Extensions.Logging;
using ONENET.Application.Common.Interfaces;

namespace ONENET.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        private readonly ICurrentUser _currentUser;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, ICurrentUser currentUser)
        {
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUser.UserId ?? "Anonymous";

            _logger.LogInformation("Handling {RequestName} request by User: {UserId}", requestName, userId);

            var response = await next();

            _logger.LogInformation("Finished {RequestName} request by User: {UserId}", requestName, userId);

            return response;
        }
    }
}