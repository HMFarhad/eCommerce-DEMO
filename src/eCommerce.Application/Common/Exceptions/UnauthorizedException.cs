using System.Net;

namespace eCommerce.Application.Common.Exceptions;

public class UnauthorizedException : eCommerceException
{
    public UnauthorizedException() : base("Unauthorized request.", HttpStatusCode.Unauthorized)
    {
    }
}
