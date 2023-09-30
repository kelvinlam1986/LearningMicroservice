using Common.Logging;
using Contracts.Common.Interfaces;
using Customer.API.Persistence;
using Customer.API.Repositories;
using Customer.API.Repositories.Interface;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Customer.API.Controllers;
using Customer.API;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SeriLogger.Configure);

try
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
    builder.Services.AddDbContext<CustomerContext>(opt => opt.UseNpgsql(connectionString));
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>()
        .AddScoped(typeof(IRepositoryBaseAsync<,,>), typeof(RepositoryBaseAsync<,,>))
        .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

    builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

    var app = builder.Build();
    Log.Information("Starting Customer API up");
   
    app.MapCustomerApi();
    
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseRouting();

    // app.UseHttpsRedirection() for Production environment
    app.UseAuthorization();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapDefaultControllerRoute();
    });

    app.SeedCustomerData()
        .Run();

}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal))
    {
        throw;
    }

    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down Customer API complete");
    Log.CloseAndFlush();
}



