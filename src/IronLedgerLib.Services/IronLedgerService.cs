using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Tudormobile.IronLedgerLib.Services;

/// <summary>
/// Provides operations and implements endpoints for the Iron Ledger service.
/// </summary>
public class IronLedgerService : IIronLedgerService
{
    private readonly ILogger _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIronLedgerSerializer _serializer;
    private readonly IAssetRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="IronLedgerService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging diagnostic information.</param>
    /// <param name="httpContextAccessor">Provides access to the current HTTP context for logging.</param>
    /// <param name="serializer">The serializer used to deserialize incoming request payloads.</param>
    /// <param name="repository">The repository used to persist and retrieve asset records.</param>
    public IronLedgerService(ILogger<IronLedgerService> logger, IHttpContextAccessor httpContextAccessor, IIronLedgerSerializer serializer, IAssetRepository repository)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _serializer = serializer;
        _repository = repository;
    }

    /// <summary>
    /// Asynchronously returns the current status of the service, including version information.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IResult"/> with the
    /// service status information.</returns>
    public Task<IResult> GetStatusAsync(CancellationToken cancellationToken)
    {
        LogApiRequest();
        return Task.FromResult(Results.Ok(new
        {
            Assembly.GetExecutingAssembly().GetName().Version,
        }));
    }

    /// <summary>
    /// Asynchronously ingests an asset by deserializing it from the provided request body stream.
    /// </summary>
    /// <param name="body">The request body stream containing the serialized asset payload.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IResult"/>
    /// indicating the outcome of the ingest operation.</returns>
    public Task<IResult> IngestAssetAsync(Stream body, CancellationToken cancellationToken)
    {
        LogApiRequest();
        return ExecuteWithBodyAsync(null, body, async payload =>
        {
            var asset = _serializer.Deserialize<AssetId>(payload);
            if (asset is null)
                return Results.BadRequest();
            var record = new AssetRecord()
            {
                Id = asset,
                Components = new SystemComponentData()
                {
                    System = ComponentData.Empty,
                    Disks = [],
                    Memory = [],
                    Processors = [],
                }
            };
            var exists = await _repository.ExistsAsync(record, cancellationToken);
            if (!exists)
            {
                await _repository.SaveAsync(record, cancellationToken);
                return Results.Ok(asset.Id);
            }
            return Results.Conflict(asset.Id);
        }, cancellationToken);
    }

    /// <summary>
    /// Asynchronously retrieves an asset by its identifier, or all asset identifiers when no ID is provided.
    /// </summary>
    /// <param name="assetId">The unique identifier of the asset to retrieve, or <see langword="null"/> to return all asset identifiers.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IResult"/>
    /// with the asset data, all asset identifiers, or a not-found result.</returns>
    public async Task<IResult> GetAssetAsync(string? assetId, CancellationToken cancellationToken)
    {
        LogApiRequest();
        if (assetId is null)
        {
            try
            {
                var all = await _repository.GetAllIdentifiersAsync(cancellationToken);
                return Results.Ok(all.ToArray());
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Failed to retrieve all asset identifiers.");
                return Results.Problem();
            }
        }
        return await ExecuteAsync(assetId, async () =>
        {
            var record = await _repository.GetAsync(assetId, cancellationToken);
            return record is null ? Results.NotFound() : Results.Ok(record);
        });
    }

    /// <inheritdoc/>
    public Task<IResult> GetComponentsAsync(string assetId, CancellationToken cancellationToken)
    {
        LogApiRequest();
        return ExecuteAsync(assetId, async () =>
        {
            var record = await _repository.GetAsync(assetId, cancellationToken);
            return record is null ? Results.NotFound() : Results.Ok(record.Components);
        });
    }

    /// <inheritdoc/>
    public Task<IResult> UpdateComponentsAsync(string assetId, Stream body, CancellationToken cancellationToken)
    {
        LogApiRequest();
        return ExecuteWithBodyAsync(assetId, body, async payload =>
        {
            var components = _serializer.Deserialize<SystemComponentData>(payload);
            var record = await _repository.GetAsync(assetId, cancellationToken);
            if (components is not null && record is not null)
            {
                var updatedRecord = record with { Components = components };
                await _repository.SaveAsync(updatedRecord, cancellationToken);
                return Results.Ok(updatedRecord.Id);
            }
            return record is null ? Results.NotFound(assetId) : Results.BadRequest();
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IResult> GetNotesAsync(string assetId, CancellationToken cancellationToken)
    {
        LogApiRequest();
        return ExecuteAsync(assetId, async () =>
        {
            var notes = await _repository.GetNotesAsync(assetId, cancellationToken);
            return Results.Ok(notes);
        });
    }

    /// <inheritdoc/>
    public Task<IResult> UpdateNotesAsync(string assetId, Stream body, CancellationToken cancellationToken)
    {
        LogApiRequest();
        return ExecuteWithBodyAsync(assetId, body, async payload =>
        {
            await _repository.SaveNotesAsync(assetId, payload, cancellationToken);
            return Results.Ok();
        }, cancellationToken);
    }

    private async Task<IResult> ExecuteAsync(string assetId, Func<Task<IResult>> operation)
    {
        if (!_repository.ValidateId(assetId))
            return Results.BadRequest();
        try
        {
            return await operation();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Repository operation failed for asset {AssetId}.", assetId);
            return Results.Problem();
        }
    }

    private async Task<IResult> ExecuteWithBodyAsync(string? assetId, Stream body, Func<string, Task<IResult>> operation, CancellationToken ct)
    {
        if (assetId is not null && !_repository.ValidateId(assetId))
            return Results.BadRequest();
        try
        {
            using var reader = new StreamReader(body, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
            var payload = await reader.ReadToEndAsync(ct);
            return await operation(payload);
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Failed to read request body for asset {AssetId}.", assetId);
            return Results.BadRequest();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Operation failed for asset {AssetId}.", assetId);
            return Results.Problem();
        }
    }

    /// <summary>
    /// Logs information about an API request using the current HTTP context.
    /// </summary>
    /// <param name="callerName">The name of the calling method, automatically populated by the compiler.</param>
    private void LogApiRequest([CallerMemberName] string callerName = "")
    {
        if (!_logger.IsEnabled(LogLevel.Information))
            return;

        _logger.LogInformation("{ServiceName}, {CallerName}, {RemoteIpAddress}",
            nameof(IronLedgerService), callerName,
            _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress);
    }

}
