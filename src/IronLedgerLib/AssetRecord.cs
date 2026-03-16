namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Represents a stored asset with its identity and component data.
/// </summary>
public record AssetRecord
{
    /// <summary>
    /// Gets the asset identifier, which provides a stable unique key derived from hardware metadata.
    /// </summary>
    public required AssetId Id { get; init; }

    /// <summary>
    /// Gets the aggregated system component data for this asset.
    /// </summary>
    public required SystemComponentData Components { get; init; }
}
