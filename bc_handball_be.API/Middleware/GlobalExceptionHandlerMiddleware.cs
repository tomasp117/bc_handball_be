using System.Net;
using System.Text.Json;
using bc_handball_be.Core.Exceptions;

namespace bc_handball_be.API.Middleware;

/// <summary>
/// Global exception handler middleware that catches all unhandled exceptions
/// and returns standardized JSON error responses.
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                notFoundEx.Message,
                null as Dictionary<string, string[]>
            ),
            BadRequestException badRequestEx => (
                HttpStatusCode.BadRequest,
                badRequestEx.Message,
                null as Dictionary<string, string[]>
            ),
            ValidationException validationEx => (
                HttpStatusCode.UnprocessableEntity,
                validationEx.Message,
                validationEx.Errors as Dictionary<string, string[]>
            ),
            UnauthorizedException unauthorizedEx => (
                HttpStatusCode.Unauthorized,
                unauthorizedEx.Message,
                null as Dictionary<string, string[]>
            ),
            ForbiddenException forbiddenEx => (
                HttpStatusCode.Forbidden,
                forbiddenEx.Message,
                null as Dictionary<string, string[]>
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "An error occurred while processing your request.",
                null as Dictionary<string, string[]>
            )
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new ErrorResponse
        {
            StatusCode = (int)statusCode,
            Message = message,
            Errors = errors
        };

        // Include stack trace in Development environment
        if (_env.IsDevelopment() && statusCode == HttpStatusCode.InternalServerError)
        {
            response.Details = exception.ToString();
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Standard error response format.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// HTTP status code.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Validation errors (if applicable).
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// Detailed error information (only in Development).
    /// </summary>
    public string? Details { get; set; }
}
