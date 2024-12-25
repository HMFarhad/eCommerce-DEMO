using Microsoft.AspNetCore.Http;
using eCommerce.Application.Common.Exceptions;
using eCommerce.Application.Common.Interfaces;
using Serilog;
using Serilog.Context;
using System.Net;

namespace eCommerce.Infrastructure.Middleware;

internal class ExceptionMiddleware : IMiddleware
{
    private const string _responseTemplate = "Response({correlationId}): {{Response Body: {Message}}}";
    private const string _defaultMessage = "Internal Server Error.";

    private readonly ISerializerService _jsonSerializer;
    private readonly string _correlationId;
    public ExceptionMiddleware(ISerializerService jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
        _correlationId = Guid.NewGuid().GetHashCode().ToString("X8");
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            using (LogContext.PushProperty("CorrelationId", _correlationId))
            {
                await next.Invoke(context);
            }
        }
        catch (Exception exception)
        {
            var baseException = exception.GetBaseException();

            (var Message, var StatusCode) = baseException switch
            {
                eCommerceException e when e.Message is not null => (e.Message, e.StatusCode),
                _ => (_defaultMessage, HttpStatusCode.InternalServerError)
            };

            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)StatusCode;
                Log.Error(_responseTemplate, _correlationId, baseException.Message);
                await context.Response.WriteAsync(_jsonSerializer.Serialize(new ErrorResult() { Message = Message }));
            }
            else
            {
                Log.Warning("Response({correlationId}): Can't write error response. Response has already started.", _correlationId);
            }
        }
    }
}