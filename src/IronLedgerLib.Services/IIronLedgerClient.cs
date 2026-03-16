namespace Tudormobile.IronLedgerLib.Services;

/// <summary>
/// Defines the contract for interacting with the IronLedger API client.
/// </summary>
public interface IIronLedgerClient
{
    /// <summary>
    /// Creates and initializes an instance of the <see cref="IIronLedgerClient"/> interface.
    /// </summary>
    /// <param name="httpClient">The HttpClient class to use.</param>
    /// <param name="logger">Optional logger instance for logging diagnostic information. If null, a NullLogger will be used.</param>
    /// <param name="serializer">Optional serializer instance for serializing and deserializing API data. If null, the internal serializer will be used.</param>
    public static IIronLedgerClient Create(HttpClient httpClient,
        ILogger? logger = null,
        IIronLedgerSerializer? serializer = null) => new IronLedgerClient(httpClient, logger, serializer);

    /// <summary>
    /// Retrieves service status from the API.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IronLedgerResponse{T}"/> containing the service status.</returns>
    Task<IronLedgerResponse<string>> GetStatusAsync(CancellationToken cancellationToken = default);
}
