using Shared.Configurations;
using Hangfire;

namespace Hangfire.API.Extensions
{
    public static class HostExtensions
    {
        public static void AddAppConfiguration(this ConfigureHostBuilder host)
        {
            host.ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            });
        }

        public static IApplicationBuilder UseHangfireDashboad(this IApplicationBuilder app, IConfiguration config)
        {
            var configDashboard = config.GetSection("HangFireSettings:Dashboard").Get<DashboardOptions>();
            var hangfireSettings = config.GetSection("HangFireSettings").Get<HangfireSettings>();
            var hangfireRoute = hangfireSettings.Route;

            app.UseHangfireDashboard(hangfireRoute, new DashboardOptions
            {
                DashboardTitle = configDashboard.DashboardTitle,
                StatsPollingInterval = configDashboard.StatsPollingInterval,
                AppPath = configDashboard.AppPath,
                IgnoreAntiforgeryToken = true
            });

            return app;        
        }
    }
}
