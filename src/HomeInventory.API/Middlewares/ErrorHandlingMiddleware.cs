namespace HomeInventory.API.Middlewares;

using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

public class ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail) = exception switch
        {
            NotFoundException => (
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                exception.Message),

            AlreadyExistsException => (
                StatusCodes.Status409Conflict,
                "Resource Conflict",
                exception.Message),

            BusinessRuleValidationException => (
                StatusCodes.Status400BadRequest,
                "Business Rule Violation",
                exception.Message),

            UnauthorizedActionException => (
                StatusCodes.Status403Forbidden,
                "Access Denied",
                exception.Message),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Critical error: {Message}", exception.Message);
        }
        else
        {
            logger.LogWarning("Domain exception: {Message} ({Type})", exception.Message, exception.GetType().Name);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = exception.GetType().Name,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}