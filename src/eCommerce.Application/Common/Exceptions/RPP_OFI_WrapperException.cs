using System.Net;

namespace eCommerce.Application.Common.Exceptions;

public class eCommerceException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public eCommerceException() { }
    public eCommerceException(string message, HttpStatusCode statusCode)
        : base(message) => StatusCode = statusCode;
}
