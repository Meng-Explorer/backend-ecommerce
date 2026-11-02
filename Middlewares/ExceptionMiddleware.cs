using System.Net;
using System.Text.Json;
using WepApi.Exceptions;
using WepApi.Helpers;

namespace WepApi.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _request;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate request, ILogger<ExceptionMiddleware> logger)
    {
        _request = request;
        _logger = logger;
    }

    private static async Task HandleException(
        HttpContext context,
        HttpStatusCode statusCode,
        string message,
        object? errors = null
    )
    {
        // If the response has already started we cannot change headers or status code.
        if (context.Response.HasStarted)
        {
            return;
        }

        try
        {
            context.Response.Clear();
        }
        catch
        {
            // Ignore clear failures
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        // Use a consistent anonymous object structure
        var response = new
        {
            success = false,
            statusCode = (int)statusCode,
            message = message,
            errors = errors ?? new List<string>(),
            data = new { }
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _request(context);
        }
        catch (BadRequestException ex)
        {
            if (!string.IsNullOrEmpty(ex.Errors))
            {
                await HandleException(context, HttpStatusCode.BadRequest, ex.Message, ex.Errors);
            }
            else
            {
                await HandleException(context, HttpStatusCode.BadRequest, ex.Message);
            }
        }
        catch (UnauthorizeException ex)
        {
            await HandleException(context, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (NotFoundException ex)
        {
            await HandleException(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (ValidationException ex)
        {
            await HandleException(context, HttpStatusCode.BadRequest, ex.Message, ex.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await HandleException(
                context,
                HttpStatusCode.InternalServerError,
                $"An unexpected error occurred. Reference: {context.TraceIdentifier}");
        }
    }
}
