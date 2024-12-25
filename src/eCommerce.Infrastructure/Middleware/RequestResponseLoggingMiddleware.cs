using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using Serilog;

namespace eCommerce.Infrastructure.Middleware;

internal class RequestResponseLoggingMiddleware
{
    private const string _requestTemplate = "Request: {{Path: [{Method}]{Path}, Request Body: {RequestBody}}}";
    private const string _responseTemplate = "Response: {{Response Body: {ResponseBody}}}";

    private static readonly string[] _redactUrl = { "tokens", "codelists", "search", "organizations", "swagger", "entities" };

    private readonly RequestDelegate _requestDelegate;
    private readonly ILogger _logger;
    private readonly RecyclableMemoryStreamManager _memoryStreamManager;

    public RequestResponseLoggingMiddleware(RequestDelegate requestDelegate, ILogger logger)
    {
        _requestDelegate = requestDelegate;
        _logger = logger;
        _memoryStreamManager = new RecyclableMemoryStreamManager();
    }

    public async Task Invoke(HttpContext context)
    {
        await RequestLogging(context);
        await ResponseLogging(context);
    }

    private async Task RequestLogging(HttpContext context)
    {
        string requestBody;

        if (_redactUrl.Any(context.Request.Path.ToString().Contains))
        {
            requestBody = "Redacted.";
        }
        else
        {
            context.Request.EnableBuffering();
            await using var requestStream = _memoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            requestBody = ReadStreamInChunks(requestStream);
            context.Request.Body.Position = 0;
        }

        _logger.Information(_requestTemplate, context.Request.Method, context.Request.Path, requestBody);
    }

    private async Task ResponseLogging(HttpContext context)
    {
        string responseBody;

        if (_redactUrl.Any(context.Request.Path.ToString().Contains))
        {
            responseBody = "Redacted.";
            await _requestDelegate(context);
        }
        else
        {
            var originalBodyStream = context.Response.Body;

            try
            {
                await using var BodyStream = _memoryStreamManager.GetStream();
                context.Response.Body = BodyStream;
                await _requestDelegate(context);
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await BodyStream.CopyToAsync(originalBodyStream);
            }
            catch (Exception)
            {
                context.Response.Body = originalBodyStream;
                throw;
            }
        }

        _logger.Information(_responseTemplate, responseBody);
    }

    private static string ReadStreamInChunks(Stream stream)
    {
        const int bufferSize = 4096;
        stream.Seek(0, SeekOrigin.Begin);
        using var textWriter = new StringWriter();
        using var reader = new StreamReader(stream);
        var readChunk = new char[bufferSize];
        int readChunkLength;
        do
        {
            readChunkLength = reader.ReadBlock(readChunk, 0, bufferSize);
            textWriter.Write(readChunk, 0, readChunkLength);
        } while (readChunkLength > 0);
        return textWriter.ToString();
    }
}
