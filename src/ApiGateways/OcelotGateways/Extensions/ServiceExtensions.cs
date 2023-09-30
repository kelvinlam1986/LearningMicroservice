using Contracts.Identity;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Polly;
using Ocelot.Cache.CacheManager;
using Shared.Configurations;
using System.Text;

namespace OcelotGateways.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServiceConfiguration(
              this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSetings = configuration.GetSection(nameof(JwtSettings))
              .Get<JwtSettings>();
            services.AddSingleton(jwtSetings);

            return services;
        }

        public static void ConfigureOcelot(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOcelot(configuration)
                .AddPolly()
                .AddCacheManager(x =>
                {
                    x.WithDictionaryHandle();
                });
            services.AddTransient<ITokenService, TokenService>();
            services.AddJwtAuthentication();
        }

        internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            var settings = services.GetOptions<JwtSettings>(nameof(JwtSettings));
            if (settings == null || string.IsNullOrEmpty(settings.Key))
            {
                throw new ArgumentNullException($"{nameof(JwtSettings)} is not configure properly");
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
            var tokenValidationParamters = new TokenValidationParameters();
            tokenValidationParamters.ValidateIssuerSigningKey = true;
            tokenValidationParamters.IssuerSigningKey = signingKey;
            tokenValidationParamters.ValidateIssuer = false;
            tokenValidationParamters.ValidateAudience = false;
            tokenValidationParamters.ValidateLifetime = false;
            tokenValidationParamters.ClockSkew = TimeSpan.Zero;
            tokenValidationParamters.RequireExpirationTime = false;

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = tokenValidationParamters;
            });

            return services;
        }

        public static void ConfigureCors(
            this IServiceCollection services, IConfiguration configuration)
        {
            var allowOrigins = configuration["AllowOrigins"];
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins(allowOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();

                });
            });

        }
    }
}
