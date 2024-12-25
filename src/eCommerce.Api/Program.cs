using eCommerce.Api;
using eCommerce.Api.Configurations;
using eCommerce.Application;
using eCommerce.Infrastructure;
using eCommerce.Infrastructure.Common;
using Serilog;

StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.AddConfigurations();

    builder.Host.UseSerilog((_, config) =>
    {
        config.WriteTo.Console()
        .ReadFrom.Configuration(builder.Configuration);
    });

    builder.Services.AddMvcBuilder();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();

    var app = builder.Build();

    app.UseInfrastructure(builder.Configuration);
    app.MapEndpoints();
    app.MapGet("/ping", () => "pong");

    app.Run();
}
catch (Exception ex)
{
    StaticLogger.EnsureInitialized();
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}