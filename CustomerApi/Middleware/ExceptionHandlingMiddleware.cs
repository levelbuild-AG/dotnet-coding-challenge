using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CustomerApi.Middleware;

// This class catches ANY unexpected error anywhere in the app,
// and turns it into a clean, easy-to-read message instead of a scary stack trace.
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Let the request continue through the app as normal
            await _next(context);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Database update conflict");

            // Try to give a specific, human reason instead of a generic one
            var message = "This request could not be saved because it conflicts with existing data.";

            var innerMessage = ex.InnerException?.Message ?? "";
            if (innerMessage.Contains("Customers.Name"))
            {
                message = "A customer with this name already exists. Please choose a different name.";
            }
            else if (innerMessage.Contains("Users.Email"))
            {
                message = "A user with this email address already exists. Please use a different email.";
            }
            else if (innerMessage.Contains("Users.DisplayName"))
            {
                message = "A user with this display name already exists. Please choose a different display name.";
            }

            await WriteError(context, StatusCodes.Status409Conflict, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteError(context, StatusCodes.Status500InternalServerError,
                "Something went wrong on our end. Please try again in a moment.");
        }
    }

    private static async Task WriteError(HttpContext context, int statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var body = JsonSerializer.Serialize(new
        {
            status = statusCode,
            error = message
        });

        await context.Response.WriteAsync(body);
    }
}