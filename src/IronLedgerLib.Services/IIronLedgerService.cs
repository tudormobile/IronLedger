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
    Task<IResult> GetStatusAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Ingests an asset from the provided request body stream.
    /// </summary>
    /// <param name="body">The request body stream containing the serialized asset payload.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IResult"/> representing the outcome of the ingest operation.</returns>
    Task<IResult> InjestAssetAsync(Stream body, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an asset by its identifier, or all asset identifiers when no ID is provided.
    /// </summary>
    /// <param name="assetId">The unique identifier of the asset to retrieve, or <see langword="null"/> to return all asset identifiers.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IResult"/> containing the asset, a not-found result, or the list of all asset identifiers.</returns>
    Task<IResult> GetAssetAsync(string? assetId, CancellationToken cancellationToken);
}
