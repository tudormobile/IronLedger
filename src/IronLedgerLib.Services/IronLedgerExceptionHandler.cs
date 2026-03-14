using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Tudormobile.IronLedgerLib.Services;

/// <summary>
/// Global exception handler that catches unhandled exceptions and returns a consistent
/// <c>{ success, error }</c> JSON envelope with an appropriate HTTP status code.
/// </summary>
internal sealed class IronLedgerExceptionHandler : IExceptionHandler
{
    private readonly ILogger<IronLedgerExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    /// <summary>
    /// Initializes a new instance of the IronLedgerExceptionHandler class using the specified logger and problem details service.
    /// </summary>
    /// <param name="logger">The logger to use for recording exception handling events. Cannot be null.</param>
    /// <param name="problemDetailsService">The service used to write RFC 7807 problem details responses.</param>
    public IronLedgerExceptionHandler(ILogger<IronLedgerExceptionHandler> logger, IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception in {ServiceName}: {Message}", nameof(IronLedgerService), exception.Message);

        var (statusCode, message) = exception switch
        {
            HttpRequestException => (StatusCodes.Status502BadGateway, "The upstream service API is unavailable."),
            TimeoutException => (StatusCodes.Status504GatewayTimeout, "The upstream service API request timed out."),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        httpContext.Response.StatusCode = statusCode;
        await _problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Detail = message
            }
        });

        return true;
    }
}