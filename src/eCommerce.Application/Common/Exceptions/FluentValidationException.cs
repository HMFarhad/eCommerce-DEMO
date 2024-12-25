using System.Net;

namespace eCommerce.Application.Common.Exceptions;

public class FluentValidationException : eCommerceException
{
    public FluentValidationException(string message) : base(message, HttpStatusCode.BadRequest)
    {
    }
}