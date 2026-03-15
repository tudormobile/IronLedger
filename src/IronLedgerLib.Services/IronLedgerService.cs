using System.Reflection;
using System.Runtime.CompilerServices;

namespace Tudormobile.IronLedgerLib.Services;

/// <summary>
/// Provides operations and implements endpoints for the Iron Ledger service.
/// </summary>
public class IronLedgerService : IIronLedgerService
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="IronLedgerService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging diagnostic information.</param>
    public IronLedgerService(ILogger<IronLedgerService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Asynchronously returns the current status of the service, including version information.
    /// </summary>
    /// <param name="context">The HTTP context for the current request. Provides access to request and response details.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IResult"/> with the
    /// service status information.</returns>
    public Task<IResult> GetStatusAsync(HttpContext context, CancellationToken cancellationToken)
    {
        LogApiRequest(context);
        return Task.FromResult(Results.Ok(new
        {
            Assembly.GetExecutingAssembly().GetName().Version,
            //success = categories.IsSuccess,
            //data = new
            //{
            //    categories = categories.IsSuccess ? categories.Data : null,
            //    message = $"Service running; {categories.Data?.Count ?? 0} categories available."
            //}
        }));
    }

    /// <summary>
    /// Logs information about an API request.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <param name="callerName">The name of the calling method, automatically populated by the compiler.</param>
    private void LogApiRequest(HttpContext context, [CallerMemberName] string callerName = "")
    {
        _logger.LogInformation("{ServiceName}, {CallerName}, {RemoteIpAddress}", nameof(IronLedgerService), callerName, context.Connection.RemoteIpAddress);
    }
}
