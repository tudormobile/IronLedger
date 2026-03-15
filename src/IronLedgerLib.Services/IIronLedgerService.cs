namespace Tudormobile.IronLedgerLib.Services;

/// <summary>
/// Defines a contract for Iron Ledger service endpoints. All methods are asynchronous.
/// </summary>
public interface IIronLedgerService
{
    /// <summary>
    /// Retrieves the service status.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IResult"/> containing the service status.</returns>
    Task<IResult> GetStatusAsync(HttpContext context, CancellationToken cancellationToken);
}
