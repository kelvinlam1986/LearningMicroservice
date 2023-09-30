using Contracts.Common.Interfaces;
using FluentValidation;
using Infrastructure.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Common.Behaviors;
using Ordering.Application.Common.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application
{
    public static class ConfigureService
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            return services
                .AddTransient<ISerializeService, SerializeService>()
                .AddScoped<ExceptionMiddleware>()
                .AddAutoMapper(Assembly.GetExecutingAssembly())
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                .AddMediatR(Assembly.GetExecutingAssembly())
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
               
        }

        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
