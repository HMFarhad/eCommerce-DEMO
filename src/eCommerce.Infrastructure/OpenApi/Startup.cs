using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using eCommerce.Infrastructure.Auth;

namespace eCommerce.Infrastructure.OpenApi;

internal static class Startup
{
    private static SwaggerSettings? settings;

    internal static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services, IConfiguration config)
    {
        settings = config.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>();
        if (settings?.Enable ?? false)
        {
            services.AddVersionedApiExplorer(o => o.SubstituteApiVersionInUrl = true);
            services.AddEndpointsApiExplorer();

            _ = services.AddOpenApiDocument((document, serviceProvider) =>
            {
                document.PostProcess = doc =>
                {
                    doc.Info.Title = settings.Title;
                    doc.Info.Version = settings.Version;
                    doc.Info.Description = settings.Description;
                    doc.Info.Contact = new()
                    {
                        Name = settings.ContactName,
                        Email = settings.ContactEmail,
                        Url = settings.ContactUrl
                    };
                };

                _ = document.AddSecurity(AuthSchemes.ApiKey, new OpenApiSecurityScheme
                {
                    Name = "X-Api-Key",
                    Description = "Input Api Key into the textbox.",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Scheme = AuthSchemes.ApiKey,
                });

                document.OperationProcessors.Add(new AuthOperationProcessor());
            });
        }

        return services;
    }

    internal static IApplicationBuilder UseOpenApiDocumentation(this IApplicationBuilder app, IConfiguration config)
    {
        if (config.GetValue<bool>("SwaggerSettings:Enable"))
        {
            app.UseOpenApi();
            app.UseSwaggerUi(options =>
            {
                options.DefaultModelsExpandDepth = -1;
                options.DocumentTitle = settings?.Title;
            });
        }

        return app;
    }

}