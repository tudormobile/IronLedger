namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Contains metadata information about an asset.
/// </summary>
public record AssetMetadata
{
    /// <summary>
    /// Gets an empty <see cref="AssetMetadata"/> instance with all properties set to empty strings.
    /// </summary>
    public static AssetMetadata Empty { get; } = new AssetMetadata
    {
        SerialNumber = string.Empty,
        Manufacturer = string.Empty,
        Product = string.Empty
    };

    /// <summary>
    /// Gets the serial number of the asset.
    /// </summary>
    public required string SerialNumber { get; init; }

    /// <summary>
    /// Gets the manufacturer of the asset.
    /// </summary>
    public required string Manufacturer { get; init; }

    /// <summary>
    /// Gets the product of the asset.
    /// </summary>
    public required string Product { get; init; }

}
