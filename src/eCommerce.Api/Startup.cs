using FluentValidation.AspNetCore;
using eCommerce.Application.Common.Exceptions;

namespace eCommerce.Api;

public static class Startup
{
    public static IMvcBuilder AddMvcBuilder(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        return services.AddControllers()
         .ConfigureApiBehaviorOptions(options =>
         {
             options.InvalidModelStateResponseFactory = actionContext =>
             {
                 var errors = string.Join(string.Empty, actionContext.ModelState.Values.Where(s => s.Errors.Count > 0)
                     .SelectMany(s => s.Errors)
                     .Select(e => e.ErrorMessage));

                 throw new InvalidModelStateException(errors);
             };
         });
    }
}
