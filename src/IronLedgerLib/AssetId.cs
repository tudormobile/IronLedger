namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Represents an asset identifier that can use different identification types.
/// </summary>
public record AssetId
{
    /// <summary>
    /// Gets the system level metadata associated with this asset.
    /// </summary>
    public required AssetMetadata SystemMetadata { get; init; }

    /// <summary>
    /// Gets the baseboard (motherboard) level metadata associated with this asset.
    /// </summary>
    public required AssetMetadata BaseBoardMetadata { get; init; }

    /// <summary>
    /// Gets the bios level metadata associated with this asset.
    /// </summary>
    public required AssetMetadata BiosMetadata { get; init; }

}
