using System.Net;

namespace eCommerce.Application.Common.Exceptions;

public class InvalidModelStateException : eCommerceException
{
    public InvalidModelStateException(string message) : base(message, HttpStatusCode.BadRequest)
    {
    }
}
