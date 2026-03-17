namespace Tudormobile.IronLedgerLib.Services;

/// <summary>
/// Defines a contract for Iron Ledger service endpoints. All methods are asynchronous.
/// </summary>
public interface IIronLedgerService
{
    /// <summary>
    /// Retrieves the service status.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IResult"/> containing the service status.</returns>
    Task<IResult> GetStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Ingests an asset from the provided request body stream.
    /// </summary>
    /// <param name="body">The request body stream containing the serialized asset payload.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IResult"/> representing the outcome of the ingest operation.</returns>
    Task<IResult> IngestAssetAsync(Stream body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an asset by its identifier, or all asset identifiers when no ID is provided.
    /// </summary>
    /// <param name="assetId">The unique identifier of the asset to retrieve, or <see langword="null"/> to return all asset identifiers.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IResult"/> containing the asset, a not-found result, or the list of all asset identifiers.</returns>
    Task<IResult> GetAssetAsync(string? assetId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves the components associated with the specified asset.
    /// </summary>
    /// <param name="assetId">The unique identifier of the asset for which to retrieve components. Cannot be null or empty.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an object representing the outcome
    /// of the retrieval, including the components if successful.</returns>
    Task<IResult> GetComponentsAsync(string assetId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates the components of the specified asset using the provided data stream.
    /// </summary>
    /// <param name="assetId">The unique identifier of the asset whose components are to be updated. Cannot be null or empty.</param>
    /// <param name="body">A stream containing the component data to update. The stream must be readable and positioned at the start of the
    /// data.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an object indicating the outcome of
    /// the update operation.</returns>
    Task<IResult> UpdateComponentsAsync(string assetId, Stream body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves the notes associated with the specified asset.
    /// </summary>
    /// <param name="assetId">The unique identifier of the asset for which to retrieve notes. Cannot be null or empty.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an object representing the outcome
    /// of the notes retrieval operation.</returns>
    Task<IResult> GetNotesAsync(string assetId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates the notes associated with the specified asset.
    /// </summary>
    /// <param name="assetId">The unique identifier of the asset whose notes are to be updated. Cannot be null or empty.</param>
    /// <param name="body">A stream containing the new notes content to be applied to the asset. The stream must be readable and positioned
    /// at the start of the content.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is None.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an object indicating the outcome of
    /// the update operation.</returns>
    Task<IResult> UpdateNotesAsync(string assetId, Stream body, CancellationToken cancellationToken = default);
}
