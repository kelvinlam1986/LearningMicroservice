using Common.Logging;
using Inventory.Grpc.Extensions;
using Inventory.Grpc.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog(SeriLogger.Configure);

Log.Information($"Start {builder.Environment.ApplicationName}");

try
{

    // Add services to the container.
    builder.Services.AddServiceConfiguration(builder.Configuration);
    builder.Services.ConfigureMongoDbClient();
    builder.Services.AddInfrastructureServices();
    builder.Services.AddGrpc();

    //builder.WebHost.ConfigureKestrel(options =>
    //{
    //    options.ListenLocalhost(5007, o => o.Protocols = HttpProtocols.Http2);
    //});

    var app = builder.Build();
    app.MapGrpcService<InventoryService>();

    
   
    // Configure the HTTP request pipeline.
    app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

    app.Run();

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
    Log.Information("Shut down Invetory Grpc API complete");
    Log.CloseAndFlush();
}