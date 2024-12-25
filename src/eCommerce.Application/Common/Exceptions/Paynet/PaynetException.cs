using System.Net;

namespace eCommerce.Application.Common.Exceptions.demo;

public class demoException : eCommerceException
{
    private static readonly string _message = "Internal server error";
    private static readonly HttpStatusCode _statusCode = HttpStatusCode.BadRequest;

    public Exception AccountEnquiryException(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            message = _message;
        throw new eCommerceException(message, _statusCode);
    }

    public Exception CreditTransferException(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            message = _message;
        throw new eCommerceException(message, _statusCode);
    }

    public Exception CreditTransferReversalException(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            message = _message;
        throw new eCommerceException(message, _statusCode);
    }

    public Exception TransactionEnquiryException(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            message = _message;
        throw new eCommerceException(message, _statusCode);
    }

    public Exception QRDomesticEnquiryException(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            message = _message;
        throw new eCommerceException(message, _statusCode);
    }

    public Exception QRDomesticPaymentException(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            message = _message;
        throw new eCommerceException(message, _statusCode);
    }
}
