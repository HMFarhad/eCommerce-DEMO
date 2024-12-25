using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using eCommerce.Infrastructure.Auth;
using eCommerce.Infrastructure.Common;
using eCommerce.Infrastructure.Middleware;
using eCommerce.Infrastructure.OpenApi;

namespace eCommerce.Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        return services
            .AddApiVersioning()
            .AddAuth()
            .AddExceptionMiddleware()
            .AddOpenApiDocumentation(config)
            .AddRouting(options => options.LowercaseUrls = true)
            .AddServices();
    }

    private static IServiceCollection AddApiVersioning(this IServiceCollection services) =>
        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder, IConfiguration config) =>
        builder
        .UseExceptionMiddleware()
        .UseRouting()
        .UseAuthentication()
        .UseAuthorization()
        .UseRequestResponseLogging(config)
        .UseOpenApiDocumentation(config);

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        _ = builder.MapControllers().RequireAuthorization();
        return builder;
    }
}