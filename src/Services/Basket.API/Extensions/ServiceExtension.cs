using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services;
using Basket.API.Services.Interfaces;
using Contracts.Common.Interfaces;
using EventBus.Message.IntegrationEvents.Intefaces;
using Grpc.Net.Client;
using Infrastructure.Common;
using Infrastructure.Configurations;
using Infrastructure.Extensions;
using Infrastructure.Policies;
using Inventory.Grpc.Protos;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Configurations;

namespace Basket.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddServiceConfiguration(
                this IServiceCollection services, IConfiguration configuration)
        {
            var busSetting = configuration.GetSection(nameof(EventBusSettings))
                .Get<EventBusSettings>();
            services.AddSingleton(busSetting);

            var cacheSetting = configuration.GetSection(nameof(CacheSettings))
                .Get<CacheSettings>();
            services.AddSingleton(cacheSetting);

            var grpcSetting = configuration.GetSection(nameof(GrpcSettings))
            .Get<GrpcSettings>();
            services.AddSingleton(grpcSetting);

            var backgroundJobSettings = configuration.GetSection(nameof(BackgroundJobSettings))
                .Get<BackgroundJobSettings>();
            services.AddSingleton(backgroundJobSettings);


            return services;
        }

        public static IServiceCollection ConfigureService(this IServiceCollection services)
        {
            return services.AddScoped<IBasketRepository, BasketRepository>()
                .AddTransient<ISerializeService, SerializeService>()
                .AddTransient<IEmailTemplateService, BasketEmailTemplateService>();
        }

        public static void ConfigureHttpClientService(this IServiceCollection services)
        {
            services.AddHttpClient<BackgroundJobHttpService>()
                .UseImmediateHttpRetryPolicy()
                .UseCircuitBreakerPolicy()
                .ConfigureTimeoutPolicy();
        }

        public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var cacheSettings = services.GetOptions<CacheSettings>("CacheSettings");
            if (string.IsNullOrEmpty(cacheSettings.ConnectionString))
            {
                throw new ArgumentException("Redis connection string is not configured");
            }

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cacheSettings.ConnectionString;
            });
        }

        public static void ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
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
                cfg.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(mqConnection);
                });

                cfg.AddRequestClient<IBasketCheckoutEvent>();
            });
        }

        public static IServiceCollection ConfigureGrpcService(this IServiceCollection services)
        {
            var settings = services.GetOptions<GrpcSettings>(nameof(GrpcSettings));
            services.AddGrpcClient<StockProtoService.StockProtoServiceClient>(
                x => x.Address = new Uri(settings.StockUrl));
            services.AddScoped<StockItemGrpcService>();
            return services;
        }

        public static void ConfigureHealthChecks(this IServiceCollection services)
        {
            var cacheSettings = services.GetOptions<CacheSettings>("CacheSettings");
            services.AddHealthChecks()
                .AddRedis(cacheSettings.ConnectionString, name: "Redis Health", failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);
        }
    }
}
