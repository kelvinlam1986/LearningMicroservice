using Contracts.Common.Interfaces;
using Contracts.Services;
using Infrastructure.Common;
using Infrastructure.Configurations;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Common.Intefaces;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Repositories;

namespace Ordering.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OrderContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString"),
                    builder => builder.MigrationsAssembly(typeof(OrderContext).Assembly.FullName));
            });

            services.AddScoped<OrderContextSeed>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddScoped<ISmtpEmailService, SmtpEmailService>();

            return services;
        }
    }
}
