using Contracts.ScheduledJobs;
using Contracts.Services;
using Hangfire.API.Services;
using Hangfire.API.Services.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.ScheduledJobs;
using Infrastructure.Services;
using Shared.Configurations;

namespace Hangfire.API.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings(
            this IServiceCollection services, IConfiguration configuration)
        {
            var hangfireSettings = configuration.GetSection(nameof(HangfireSettings))
                .Get<HangfireSettings>();

            services.AddSingleton(hangfireSettings);

            var emailSetting = configuration.GetSection(nameof(SMTPEmailSettings))
                .Get<SMTPEmailSettings>();
            services.AddSingleton(emailSetting);
            return services;
        }

        internal static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            return services.AddTransient<IScheduledJobService, HangfireService>()
                            .AddTransient<IBackgroundJobService, BackgroundJobService>()
                            .AddTransient<ISmtpEmailService, SmtpEmailService>();
        }
    }
}
