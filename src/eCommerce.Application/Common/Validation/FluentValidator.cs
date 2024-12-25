using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using eCommerce.Application.Common.Exceptions;

namespace eCommerce.Application.Common.Validation;

public class FluentValidator<T> : AbstractValidator<T>, IValidatorInterceptor
{
    public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result)
    {
        if (!result.IsValid && result.Errors.Count > 0)
        {
            throw new FluentValidationException(result.Errors.FirstOrDefault()?.ErrorMessage
                ?? "One or more validation errors occurred.");
        }

        return result;
    }

    public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext)
    {
        return commonContext;
    }
}