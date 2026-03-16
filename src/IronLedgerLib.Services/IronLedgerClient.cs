
using Microsoft.Extensions.Logging.Abstractions;
using Tudormobile.IronLedgerLib.Serialization;

namespace Tudormobile.IronLedgerLib.Services;

/// <summary>
/// Provides methods for interacting with an IronLedger service over HTTP.
/// </summary>
public class IronLedgerClient : IIronLedgerClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly IIronLedgerSerializer _serializer;

    /// <summary>
    /// Initializes a new instance of the IronLedgerClient class with the specified HTTP client, logger, and serializer.
    /// </summary>
    /// <remarks>This constructor allows customization of logging and serialization behavior. If no logger or
    /// serializer is provided, default implementations are used.</remarks>
    /// <param name="httpClient">The HTTP client used to send requests to the IronLedger service. Cannot be null.</param>
    /// <param name="logger">The logger used for diagnostic and error messages. If null, a no-op logger is used.</param>
    /// <param name="serializer">The serializer used for request and response payloads. If null, a default JSON serializer is used.</param>
    public IronLedgerClient(HttpClient httpClient, ILogger? logger = null, IIronLedgerSerializer? serializer = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));
        this._httpClient = httpClient;
        this._logger = logger ?? new NullLogger<IronLedgerClient>();
        this._serializer = serializer ?? new IronLedgerJsonSerializer();
    }

    /// <inheritdoc/>
    public async Task<IronLedgerResponse<string>> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var reply = await _httpClient.GetStringAsync("api/v1/status", cancellationToken);
            return reply == null
                ? IronLedgerResponse<string>.Failure("Unable to retrieve version")
                : IronLedgerResponse<string>.Success(reply);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving IronLedger status.");
            return IronLedgerResponse<string>.Failure(ex.Message);
        }
    }
}
