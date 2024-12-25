using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using eCommerce.Infrastructure.Auth;
using System.Reflection;

namespace eCommerce.Infrastructure.OpenApi;

public class AuthOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var list = ((AspNetCoreOperationProcessorContext)context).ApiDescription?.ActionDescriptor?.TryGetPropertyValue<IList<object>>("EndpointMetadata");

        if (list is not null)
        {
            if (list.OfType<AuthorizeAttribute>().Any(a => a.AuthenticationSchemes == AuthSchemes.ApiKey))
            {
                SetSecurityDescription(context, AuthSchemes.ApiKey);
                return true;
            }

            if (list.OfType<AllowAnonymousAttribute>().Any())
            {
                return true;
            }
        }

        return true;
    }

    private static void SetSecurityDescription(OperationProcessorContext context, string authScheme)
    {
        (context.OperationDescription.Operation.Security ??= new List<OpenApiSecurityRequirement>()).Add(new OpenApiSecurityRequirement
        {
            {
                authScheme,
                Array.Empty<string>()
            }
        });
    }
}

internal static class ObjectExtensions
{
    public static T? TryGetPropertyValue<T>(this object obj, string propertyName)
    {
        return obj.GetType().GetRuntimeProperty(propertyName) is PropertyInfo propertyInfo
            ? (T?)propertyInfo.GetValue(obj)
            : default;
    }
}


