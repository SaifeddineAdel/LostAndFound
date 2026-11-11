using LostAndFound.Application.Common.Exceptions;
using LostAndFound.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace LostAndFound.Api.Common.ExceptionHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        int statusCode;
        string message;

        if (exception is LostAndFoundException appException)
        {
            statusCode = appException.StatusCode;
            message = appException.Message;

            _logger.LogWarning(
                exception,
                "Application exception occurred.");
        }
        else if (exception is DomainException domainException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            message = domainException.Message;

            _logger.LogWarning(
                exception,
                "Domain exception occurred.");
        }
        else
        {
            statusCode = StatusCodes.Status500InternalServerError;
            message = "An unexpected error occurred.";

            _logger.LogError(
                exception,
                "Unhandled exception occurred.");
        }

        var response = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = message
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            response,
            cancellationToken);

        return true;
    }
}