using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Ordering.Application.Common.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly Stopwatch _stopwatch;
        private readonly ILogger<TRequest> _logger;

        public PerformanceBehavior(
            ILogger<TRequest> logger)
        {
            _stopwatch = new Stopwatch();
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request,
            RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _stopwatch.Start();
            var respone = await next();
            _stopwatch.Stop();

            var elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
            if (elapsedMilliseconds <= 500)
            {
                return respone;
            }

            var requestName = typeof(TRequest).Name;
            _logger.LogWarning("Application long running request: {Name} {ElapsedMilliseconds} {@Request}",
                requestName, elapsedMilliseconds, request);
            return respone;
        }
    }
}
