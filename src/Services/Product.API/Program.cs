using Common.Logging;
using Product.API.Extensions;
using Product.API.Persistence;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SeriLogger.Configure);

try
{
    builder.Host.AddAppConfiguration();
    builder.Services.AddServiceConfiguration(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration);


    var app = builder.Build();
    Log.Information("Starting Product API up");

    app.UseInfrastructure();
    app.MigrateDatabase<ProductContext>((context, _) =>
    {
        ProductContextSeed.SeedProductAsync(context, Log.Logger).Wait();
    }).Run();

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
    Log.Information("Shut down Product API complete");
    Log.CloseAndFlush();
}



