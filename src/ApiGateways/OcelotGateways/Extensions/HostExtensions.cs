using Common.Logging;
using Serilog;

namespace OcelotGateways.Extensions
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
                    .AddJsonFile($"ocelot.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            }).UseSerilog(SeriLogger.Configure);
        }
    }
}
