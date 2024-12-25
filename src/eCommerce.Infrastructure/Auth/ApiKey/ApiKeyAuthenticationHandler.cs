using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using eCommerce.Application.Common.Exceptions;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace eCommerce.Infrastructure.Auth.ApiKey;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string ApiKeyHeader = "x-api-key";
    private const string Name = "System";

    private readonly ApiKeySettings _apiKeySettings;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IOptionsMonitor<ApiKeySettings> apiKeySettings) : base(options, logger, encoder, clock)
    {
        _apiKeySettings = apiKeySettings.CurrentValue;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyHeader, out var apiKeyHeaderValues))
        {
            throw new UnauthorizedException();
        }

        var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(providedApiKey) ||
            (!(providedApiKey.Equals(_apiKeySettings.Key, StringComparison.InvariantCultureIgnoreCase))
                && !(providedApiKey.Equals(_apiKeySettings.OneRegister, StringComparison.InvariantCultureIgnoreCase))))
        {
            throw new UnauthorizedException();
        }

        var claims = new Claim[] { new(ClaimTypes.Name, Name) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, AuthSchemes.ApiKey));
        var ticket = new AuthenticationTicket(principal, AuthSchemes.ApiKey);

        return await Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

