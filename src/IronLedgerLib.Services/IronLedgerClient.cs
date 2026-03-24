
using Microsoft.Extensions.Logging.Abstractions;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Tudormobile.IronLedgerLib.Serialization;

namespace Tudormobile.IronLedgerLib.Services;

/// <summary>
/// Provides methods for interacting with an IronLedger service over HTTP.
/// </summary>
internal class IronLedgerClient : IIronLedgerClient
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
        ArgumentNullException.ThrowIfNull(httpClient.BaseAddress, "httpClient.BaseAddress");
        httpClient.BaseAddress = new Uri(httpClient.BaseAddress.ToString().TrimEnd('/') + "/");
        _httpClient = httpClient;
        _logger = logger ?? new NullLogger<IronLedgerClient>();
        _serializer = serializer ?? new IronLedgerJsonSerializer();
        _logger.LogDebug("{ClientName} initialized; BaseAddress={BaseAddress}.", nameof(IronLedgerClient), _httpClient.BaseAddress);
    }

    /// <inheritdoc/>
    public Task<IronLedgerResponse<string>> CreateAssetAsync(AssetId assetId, CancellationToken cancellationToken = default)
    {
        LogRequest();
        ArgumentNullException.ThrowIfNull(assetId, nameof(assetId));
        return ExecuteAsync<string>(
            ct =>
            {
                var content = new StringContent(_serializer.Serialize(assetId), new MediaTypeHeaderValue(_serializer.ContentType));
                return _httpClient.PostAsync("api/v1/assets/ingest", content, ct);
            },
            nameof(CreateAssetAsync),
            cancellationToken,
            validate: returnedId => returnedId == assetId.Id
                ? IronLedgerResponse<string>.Success(assetId.Id)
                : IronLedgerResponse<string>.Failure($"Response ID '{returnedId}' does not match expected '{assetId.Id}'."));
    }

    /// <inheritdoc/>
    public Task<IronLedgerResponse<AssetRecord>> GetAssetAsync(string assetIdString, CancellationToken cancellationToken = default)
    {
        LogRequest();
        ArgumentNullException.ThrowIfNullOrEmpty(assetIdString, nameof(assetIdString));
        return ExecuteAsync<AssetRecord>(
            ct => _httpClient.GetAsync($"api/v1/assets/{assetIdString}", ct),
            nameof(GetAssetAsync),
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IronLedgerResponse<List<string>>> GetAssetIdsAsync(CancellationToken cancellationToken = default)
    {
        LogRequest();
        return ExecuteAsync<List<string>>(
            ct => _httpClient.GetAsync("api/v1/assets/", ct),
            nameof(GetAssetIdsAsync),
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IronLedgerResponse<string>> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        LogRequest();
        return ExecuteAsync<string>(
            ct => _httpClient.GetAsync("api/v1/status", ct),
            nameof(GetStatusAsync),
            cancellationToken,
            deserialize: body => body);
    }

    /// <inheritdoc/>
    public Task<IronLedgerResponse<SystemComponentData>> GetComponentsAsync(string assetIdString, CancellationToken cancellationToken = default)
    {
        LogRequest();
        ArgumentNullException.ThrowIfNullOrEmpty(assetIdString, nameof(assetIdString));
        return ExecuteAsync<SystemComponentData>(
           ct => _httpClient.GetAsync($"api/v1/assets/{assetIdString}/components", ct),
           nameof(GetComponentsAsync),
           cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IronLedgerResponse<string>> SetComponentsAsync(string assetIdString, SystemComponentData components, CancellationToken cancellationToken = default)
    {
        LogRequest();
        ArgumentNullException.ThrowIfNull(assetIdString, nameof(assetIdString));
        return ExecuteAsync<string>(
            ct =>
            {
                var content = new StringContent(_serializer.Serialize(components), new MediaTypeHeaderValue(_serializer.ContentType));
                return _httpClient.PatchAsync($"api/v1/assets/{assetIdString}/components", content, ct);
            },
            nameof(SetNotesAsync),
            cancellationToken,
            validate: returnedId => returnedId == assetIdString
                ? IronLedgerResponse<string>.Success(assetIdString)
                : IronLedgerResponse<string>.Failure($"Response ID '{returnedId}' does not match expected '{assetIdString}'."));
    }

    /// <inheritdoc/>
    public Task<IronLedgerResponse<string>> GetNotesAsync(string assetIdString, CancellationToken cancellationToken = default)
    {
        LogRequest();
        ArgumentNullException.ThrowIfNullOrEmpty(assetIdString, nameof(assetIdString));
        return ExecuteAsync<string>(
           ct => _httpClient.GetAsync($"api/v1/assets/{assetIdString}/notes", ct),
           nameof(GetNotesAsync),
           cancellationToken,
           deserialize: body => body);
    }

    /// <inheritdoc/>
    public Task<IronLedgerResponse<string>> SetNotesAsync(string assetIdString, string notes, CancellationToken cancellationToken = default)
    {
        LogRequest();
        ArgumentNullException.ThrowIfNullOrEmpty(assetIdString, nameof(assetIdString));
        return ExecuteAsync<string>(
            ct =>
            {
                var content = new StringContent(notes);
                return _httpClient.PatchAsync($"api/v1/assets/{assetIdString}/notes", content, ct);
            },
            nameof(SetNotesAsync),
            cancellationToken,
            validate: returnedId => returnedId == assetIdString
                ? IronLedgerResponse<string>.Success(assetIdString)
                : IronLedgerResponse<string>.Failure($"Response ID '{returnedId}' does not match expected '{assetIdString}'."));
    }

    private void LogRequest([CallerMemberName] string callerName = "")
    {
        if (!_logger.IsEnabled(LogLevel.Information))
            return;

        _logger.LogInformation("{ClientName}, {CallerName}, {BaseAddress}",
            nameof(IronLedgerClient), callerName, _httpClient.BaseAddress);
    }

    private async Task<IronLedgerResponse<T>> ExecuteAsync<T>(
        Func<CancellationToken, Task<HttpResponseMessage>> requestFactory,
        string operationName,
        CancellationToken cancellationToken,
        Func<string, T?>? deserialize = null,
        Func<T, IronLedgerResponse<T>>? validate = null)
    {
        try
        {
            T? result;
            try
            {
                using var response = await requestFactory(cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return IronLedgerResponse<T>.Failure($"Unexpected response: {(int)response.StatusCode} {response.ReasonPhrase}");

                _logger.LogDebug("{Operation} received HTTP {StatusCode}.", operationName, (int)response.StatusCode);

                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                result = deserialize != null ? deserialize(body) : _serializer.Deserialize<T>(body);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed during {Operation}.", operationName);
                return IronLedgerResponse<T>.Failure($"HTTP request failed: {ex.Message}");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize response during {Operation}.", operationName);
                return IronLedgerResponse<T>.Failure("Failed to deserialize response.");
            }

            if (result is null)
                return IronLedgerResponse<T>.Failure("Response contained no data.");

            return validate is not null ? validate(result) : IronLedgerResponse<T>.Success(result);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "{Operation} was cancelled.", operationName);
            return IronLedgerResponse<T>.Failure("The operation was cancelled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during {Operation}.", operationName);
            return IronLedgerResponse<T>.Failure("Unexpected error occurred.");
        }
    }
}
