﻿using Infrastructure.Extensions;
using Inventory.Grpc.Repositories;
using Inventory.Grpc.Repositories.Interfaces;
using MongoDB.Driver;
using Shared.Configurations;

namespace Inventory.Grpc.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServiceConfiguration(
              this IServiceCollection services, IConfiguration configuration)
        {
            var databaseSetting = configuration.GetSection(nameof(MongoDbSettings))
                .Get<MongoDbSettings>();
            services.AddSingleton(databaseSetting);

            return services;
        }

        private static string GetConnectionString(this IServiceCollection services)
        {
            var settings = services.GetOptions<MongoDbSettings>(nameof(MongoDbSettings));
            if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
            {
                throw new ArgumentNullException("Database settings is not configured");
            }

            var databaseName = settings.DatabaseName;
            var mongoConnectionString = settings.ConnectionString + "/" + databaseName + "?authSource=admin";
            return mongoConnectionString;
        }

        public static void ConfigureMongoDbClient(this IServiceCollection services)
        {
            services.AddSingleton<IMongoClient>(new MongoClient(GetConnectionString(services)))
                .AddScoped(x => x.GetService<IMongoClient>().StartSession());
        }

        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IIventoryRepository, InventoryRepository>();
        }
    }
}
