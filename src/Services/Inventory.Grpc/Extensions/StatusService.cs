
using Grpc.HealthCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Hangfire.API.Extensions
{
    public class StatusService : BackgroundService
    {
        private readonly HealthServiceImpl _healthService;
        private readonly HealthCheckService _healthCheckService;

        public StatusService(HealthServiceImpl healthService, HealthCheckService healthCheckService)
        {
            _healthService = healthService;
            _healthCheckService = healthCheckService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var health = await _healthCheckService.CheckHealthAsync(stoppingToken);

                foreach (var h in health.Entries) 
                {
                    _healthService.SetStatus(h.Key,
                        health.Status == HealthStatus.Healthy
                        ? Grpc.Health.V1.HealthCheckResponse.Types.ServingStatus.Serving
                        : Grpc.Health.V1.HealthCheckResponse.Types.ServingStatus.NotServing
                        );

                    await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
                }
            }
        }
    }
}
