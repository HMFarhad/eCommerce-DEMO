using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.Infrastructure.Middleware;

internal static class Startup
{
    internal static IServiceCollection AddExceptionMiddleware(this IServiceCollection services) =>
        services.AddScoped<ExceptionMiddleware>();

    internal static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionMiddleware>();

    internal static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app, IConfiguration config)
    {
        if (GetMiddlewareSettings(config)?.EnableHttpsLogging ?? false)
        {
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }

        return app;
    }

    private static MiddlewareSettings? GetMiddlewareSettings(IConfiguration config) =>
        config.GetSection(nameof(MiddlewareSettings)).Get<MiddlewareSettings>();
}
