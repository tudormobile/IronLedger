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

    /// <summary>
    /// Asynchronously retrieves the components associated with the specified asset.
    /// </summary>
    /// <param name="assetIdString">The unique identifier of the asset for which to retrieve notes. Cannot be null or empty.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IronLedgerResponse with the components
    /// returns as SystemComponentData.</returns>
    Task<IronLedgerResponse<SystemComponentData>> GetComponentsAsync(string assetIdString, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves the components associated with the specified asset.
    /// </summary>
    /// <param name="assetId">The identifier of the asset for which to retrieve notes.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IronLedgerResponse with the components
    /// returns as SystemComponentData.</returns>
    Task<IronLedgerResponse<SystemComponentData>> GetComponentsAsync(AssetId assetId, CancellationToken cancellationToken = default) => GetComponentsAsync(assetId.Id, cancellationToken);

    /// <summary>
    /// Asynchronously sets the notes for the specified asset.
    /// </summary>
    /// <param name="assetIdString">The unique identifier of the asset for which to set notes. Cannot be null or empty.</param>
    /// <param name="components">The components to associate with the asset.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IronLedgerResponse with the
    /// string identifier of the updated asset.</returns>
    Task<IronLedgerResponse<string>> SetComponentsAsync(string assetIdString, SystemComponentData components, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously sets the notes for the specified asset.
    /// </summary>
    /// <param name="assetId">The identifier of the asset for which to set notes. Cannot be null or empty.</param>
    /// <param name="components">The notes to associate with the asset.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IronLedgerResponse with the
    /// string identifier of the updated asset.</returns>
    Task<IronLedgerResponse<string>> SetComponentsAsync(AssetId assetId, SystemComponentData components, CancellationToken cancellationToken = default) => SetComponentsAsync(assetId.Id, components, cancellationToken);

    /// <summary>
    /// Asynchronously retrieves the notes associated with the specified asset.
    /// </summary>
    /// <param name="assetIdString">The unique identifier of the asset for which to retrieve notes. Cannot be null or empty.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IronLedgerResponse with the notes
    /// as a string. If no notes are found, the response may contain an empty string.</returns>
    Task<IronLedgerResponse<string>> GetNotesAsync(string assetIdString, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves the notes associated with the specified asset.
    /// </summary>
    /// <param name="assetId">The identifier of the asset for which to retrieve notes.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IronLedgerResponse with the notes
    /// for the specified asset as a string.</returns>
    Task<IronLedgerResponse<string>> GetNotesAsync(AssetId assetId, CancellationToken cancellationToken = default) => GetNotesAsync(assetId.Id, cancellationToken);

    /// <summary>
    /// Asynchronously sets the notes for the specified asset.
    /// </summary>
    /// <param name="assetIdString">The unique identifier of the asset for which to set notes. Cannot be null or empty.</param>
    /// <param name="notes">The notes to associate with the asset. May be an empty string to clear existing notes.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IronLedgerResponse with the
    /// string identifier of the updated asset.</returns>
    Task<IronLedgerResponse<string>> SetNotesAsync(string assetIdString, string notes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously sets the notes for the specified asset.
    /// </summary>
    /// <param name="assetId">The identifier of the asset for which to set notes. Cannot be null or empty.</param>
    /// <param name="notes">The notes to associate with the asset. May be an empty string to clear existing notes.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IronLedgerResponse with the
    /// string identifier of the updated asset.</returns>
    Task<IronLedgerResponse<string>> SetNotesAsync(AssetId assetId, string notes, CancellationToken cancellationToken = default) => SetNotesAsync(assetId.Id, notes, cancellationToken);

}
