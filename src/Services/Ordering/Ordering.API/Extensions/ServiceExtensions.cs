using Infrastructure.Configurations;
using Infrastructure.Extensions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ordering.API.Application.IntegrationEvents.EventHandlers;
using Shared.Configurations;


namespace Ordering.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServiceConfiguration(
            this IServiceCollection services, IConfiguration configuration) 
        {
            var emailSetting = configuration.GetSection(nameof(SMTPEmailSettings))
                .Get<SMTPEmailSettings>();
            services.AddSingleton(emailSetting);

            var eventBusSetting = configuration.GetSection(nameof(EventBusSettings))
                .Get<EventBusSettings>();
            services.AddSingleton(eventBusSetting);
            return services;
        }

        public static void ConfigureMassTransit(this IServiceCollection services)
        {
            var settings = services.GetOptions<EventBusSettings>("EventBusSettings");
            if (settings is null || string.IsNullOrEmpty(settings.HostAddress))
            {
                throw new ArgumentException("Event Bus is not configured");
            }

            var mqConnection = new Uri(settings.HostAddress);
            services.TryAddSingleton(MassTransit.KebabCaseEndpointNameFormatter.Instance);
            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumersFromNamespaceContaining<BasketCheckoutHandler>();
                cfg.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(mqConnection);
                    cfg.ConfigureEndpoints(ctx);
                });
            });
        }
    }
}
