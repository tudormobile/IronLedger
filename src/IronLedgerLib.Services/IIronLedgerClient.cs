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

    /// <summary>
    /// Asynchronously creates a new asset with the specified identifier.
    /// </summary>
    /// <param name="assetId">The unique identifier for the asset to be created. Cannot be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an IronLedgerResponse with the
    /// identifier of the newly created asset.
    /// </returns>
    Task<IronLedgerResponse<string>> CreateAssetAsync(AssetId assetId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves the asset record associated with the specified asset identifier.
    /// </summary>
    /// <param name="assetIdString">The unique identifier of the asset to retrieve. Cannot be null or empty.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IronLedgerResponse object with
    /// the asset record if found; otherwise, the response indicates the error or not found status.</returns>
    Task<IronLedgerResponse<AssetRecord>> GetAssetAsync(string assetIdString, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the asset record associated with the specified asset identifier asynchronously.
    /// </summary>
    /// <param name="assetId">The identifier of the asset to retrieve. Must not be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IronLedgerResponse with the asset
    /// record if found.</returns>
    Task<IronLedgerResponse<AssetRecord>> GetAssetAsync(AssetId assetId, CancellationToken cancellationToken = default) => GetAssetAsync(assetId.Id, cancellationToken);

    /// <summary>
    /// Asynchronously retrieves a list of asset identifiers associated with the specified asset.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IronLedgerResponse with a list of
    /// asset IDs. The list will be empty if no assets are found.</returns>
    Task<IronLedgerResponse<List<string>>> GetAssetIdsAsync(CancellationToken cancellationToken = default);

}
