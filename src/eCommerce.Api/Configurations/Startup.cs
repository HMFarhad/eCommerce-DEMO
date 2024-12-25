namespace eCommerce.Api.Configurations;

internal static class Startup
{
    private const string configurationsDirectory = "Configurations";

    internal static WebApplicationBuilder AddConfigurations(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .AddJsonFile($"{configurationsDirectory}/logger.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"{configurationsDirectory}/middleware.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"{configurationsDirectory}/security.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"{configurationsDirectory}/encryption.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"{configurationsDirectory}/openapi.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"{configurationsDirectory}/cache.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
        return builder;
    }
}
