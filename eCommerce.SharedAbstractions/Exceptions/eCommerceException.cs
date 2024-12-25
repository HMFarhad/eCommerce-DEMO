using System.Net;

namespace eCommerce.SharedAbstractions.Exceptions;

public class eCommerceException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public eCommerceException() { }

    public eCommerceException(string message, HttpStatusCode statusCode)
        : base(message) => StatusCode = statusCode;
}
