using System.Net;

namespace eCommerce.Infrastructure.Middleware;

public class ErrorResult
{
    public ErrorResult()
    {

    }

    public ErrorResult(HttpStatusCode statusCode)
    {
        Message = statusCode switch
        {
            HttpStatusCode s when s.Equals(HttpStatusCode.BadRequest) => "The request is incorrect or corrupt, and the server can't understand it.",
            HttpStatusCode s when s.Equals(HttpStatusCode.Unauthorized) => "The request lacks valid permission for the target resource.",
            _ => "An unspecified error occurs"
        };
    }

    public string Message { get; set; } = string.Empty;
}
