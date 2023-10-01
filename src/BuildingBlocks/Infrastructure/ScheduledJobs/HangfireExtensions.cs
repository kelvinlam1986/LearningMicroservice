using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json;
using Shared.Configurations;

namespace Infrastructure.ScheduledJobs
{
    public static class HangfireExtensions
    {
        public static IServiceCollection AddHangfireService(this IServiceCollection services)
        {
            var settings = services.GetOptions<HangfireSettings>("HangFireSettings");
            if (settings == null || settings.Storage == null ||
                 string.IsNullOrEmpty(settings.Storage.ConnectionString))
            {
                throw new Exception("HangfireSettings is not configured properly");
            }

            services.ConfigureHangfireService(settings);
            services.AddHangfireServer(opt =>
            {
                opt.ServerName = settings.ServerName;
            });

            return services;
        }

        private static IServiceCollection ConfigureHangfireService(
            this IServiceCollection services, HangfireSettings settings)
        {
            if (string.IsNullOrEmpty(settings.Storage.DBProvider))
            {
                throw new Exception("DBProvider is not configured properly");
            }

            switch (settings.Storage.DBProvider.ToLower())
            {
                case "mongodb":
                    var mongoUrlBuilder = new MongoUrlBuilder(settings.Storage.ConnectionString);
                    mongoUrlBuilder.AuthenticationSource = "admin";
                    var mongoClientSettings = MongoClientSettings.FromUrl(
                        new MongoUrl(settings.Storage.ConnectionString));
                    mongoClientSettings.SslSettings = new SslSettings
                    {
                        EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
                    };

                    var mongoClient = new MongoClient(mongoClientSettings);
                    var mongoStorageOptions = new MongoStorageOptions
                    {
                        MigrationOptions = new MongoMigrationOptions
                        {
                            MigrationStrategy = new MigrateMongoMigrationStrategy(),
                            BackupStrategy = new CollectionMongoBackupStrategy()
                        },
                        CheckConnection = true,
                        Prefix = "SchedulerQueue",
                        CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
                    };

                    services.AddHangfire((provider, config) =>
                    {
                        config.UseSimpleAssemblyNameTypeSerializer()
                            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                            .UseRecommendedSerializerSettings()
                            .UseConsole()
                            .UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName, mongoStorageOptions);

                        var jsonSettings = new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All
                        };

                        config.UseSerializerSettings(jsonSettings);
                    });
                    services.AddHangfireConsoleExtensions();
                    break;

                case "postgresql":
                    break;

                case "mssql":
                    break;

                default:
                    throw new Exception($"Hangfire Db Provider {settings.Storage.DBProvider} is not supported");
            }

            return services;
        }
    }
}
