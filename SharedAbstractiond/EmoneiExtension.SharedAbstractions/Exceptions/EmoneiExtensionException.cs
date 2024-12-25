using System.Net;

namespace EmoneiExtension.SharedAbstractions.Exceptions;

public class EmoneiExtensionException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public EmoneiExtensionException() { }

    public EmoneiExtensionException(string message, HttpStatusCode statusCode)
        : base(message) => StatusCode = statusCode;
}
