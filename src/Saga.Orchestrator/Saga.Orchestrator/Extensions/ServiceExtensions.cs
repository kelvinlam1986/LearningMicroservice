using Contracts.Sagas.OrderManager;
using Infrastructure.Policies;
using Saga.Orchestrator.HttpRepository;
using Saga.Orchestrator.HttpRepository.Intefaces;
using Saga.Orchestrator.OrderManager;
using Saga.Orchestrator.Services;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTO.Baskets;

namespace Saga.Orchestrator.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
           return  services.AddTransient<ICheckoutService, CheckoutService>()
                        .AddTransient<ISagaOrderManager<BasketCheckoutDto, OrderResponse>, SagaOrderManager>();
        }

        public static IServiceCollection ConfigureHttpRepositories(this IServiceCollection services)
        {
            return services.AddScoped<IOrderHttpRepository, OrderHttpRepository>()
                            .AddScoped<IBasketHttpRepository, BasketHttpRepository>()
                            .AddScoped<IInventoryHttpRepository, InventoryHttpRepository>();   
        }

        public static void ConfigureHttpClient(this IServiceCollection services)
        {
            ConfigureOrderHttpClient(services);
            ConfigureBasketHttpClient(services);
            ConfigureInventoryHttpClient(services);
        }

        private static void ConfigureOrderHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient<IOrderHttpRepository, OrderHttpRepository>("OrdersApi", (sp, cl) =>
            {
                cl.BaseAddress = new Uri("http://localhost:5005/api/v1/");
            }).UseExponentialHttpRetryPolicy();

            services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("OrdersApi"));
        }

        private static void ConfigureBasketHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient<IBasketHttpRepository, BasketHttpRepository>("BasketsApi", (sp, cl) =>
            {
                cl.BaseAddress = new Uri("http://localhost:5004/api/");
            }).UseImmediateHttpRetryPolicy();

            services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("BasketsApi"));
        }

        private static void ConfigureInventoryHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient<IInventoryHttpRepository, InventoryHttpRepository>("InventoryApi", (sp, cl) =>
            {
                cl.BaseAddress = new Uri("http://localhost:5006/api/");
            }).UseExponentialHttpRetryPolicy();

            services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("InventoryApi"));
        }
    }
}
