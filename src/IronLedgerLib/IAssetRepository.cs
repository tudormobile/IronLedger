namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Defines a repository for persisting and retrieving <see cref="AssetRecord"/> instances
/// along with associated markdown notes.
/// </summary>
public interface IAssetRepository
{
    /// <summary>
    /// Asynchronously retrieves all available asset identifiers.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A read-only list of strings containing all identifiers. The list will be empty if no identifiers are available.</returns>
    Task<IReadOnlyList<string>> GetAllIdentifiersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all stored asset records.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A read-only list of all stored <see cref="AssetRecord"/> instances.</returns>
    Task<IReadOnlyList<AssetRecord>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single asset record by its identifier.
    /// </summary>
    /// <param name="assetId">The unique asset identifier (see <see cref="AssetId.Id"/>).</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The <see cref="AssetRecord"/> if found; otherwise <see langword="null"/>.</returns>
    Task<AssetRecord?> GetAsync(string assetId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists an asset record, creating or overwriting any existing record with the same identifier.
    /// </summary>
    /// <param name="asset">The asset record to save.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    Task SaveAsync(AssetRecord asset, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determine if a record currently exists in the repository.
    /// </summary>
    /// <param name="asset">The asset record to check.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    Task<bool> Exists(AssetRecord asset, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the asset record and its associated notes for the given identifier.
    /// </summary>
    /// <param name="assetId">The unique asset identifier to delete.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    Task DeleteAsync(string assetId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the markdown notes associated with an asset.
    /// </summary>
    /// <param name="assetId">The unique asset identifier.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The markdown text, or <see cref="string.Empty"/> if no notes exist.</returns>
    Task<string> GetNotesAsync(string assetId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists markdown notes for an asset, creating or overwriting any existing notes.
    /// </summary>
    /// <param name="assetId">The unique asset identifier.</param>
    /// <param name="markdown">The markdown text to save.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    Task SaveNotesAsync(string assetId, string markdown, CancellationToken cancellationToken = default);
}
