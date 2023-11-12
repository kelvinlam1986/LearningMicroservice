using Basket.API;
using Basket.API.Extensions;
using Common.Logging;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SeriLogger.Configure);

try
{
    builder.Host.AddAppConfiguration();
    builder.Services.AddServiceConfiguration(builder.Configuration);
    builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
    builder.Services.ConfigureService();
    builder.Services.ConfigureHttpClientService();
    builder.Services.ConfigureRedis(builder.Configuration);
    builder.Services.ConfigureGrpcService();
    builder.Services.Configure<RouteOptions>(options =>
    {
        options.LowercaseUrls = true;
    });

    // Configure MassTransit
    builder.Services.ConfigureMassTransit(builder.Configuration);

    // Add services to the container.
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();
    Log.Information("Starting Basket API up");

    // Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseRouting();
    // app.UseHttpsRedirection();

    app.UseAuthorization();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapDefaultControllerRoute();
    });

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down Basket API complete");
    Log.CloseAndFlush();
}



