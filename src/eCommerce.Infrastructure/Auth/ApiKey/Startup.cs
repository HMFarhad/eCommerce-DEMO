using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.Infrastructure.Auth.ApiKey;

internal static class Startup
{
    internal static IServiceCollection AddApiKeyAuth(this IServiceCollection services)
    {
        _ = services.AddOptions<ApiKeySettings>()
            .BindConfiguration(ApiKeySettings.ConfigSectionPath)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder authenticationBuilder, Action<AuthenticationSchemeOptions> options)
    {
        return authenticationBuilder.AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(AuthSchemes.ApiKey, options);
    }
}
