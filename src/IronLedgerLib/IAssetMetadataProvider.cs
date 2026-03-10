namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Defines a provider that retrieves asset metadata from a specific hardware component.
/// </summary>
public interface IAssetMetadataProvider
{
    /// <summary>
    /// Retrieves the asset metadata from the hardware component.
    /// </summary>
    /// <returns>The asset metadata containing serial number, manufacturer, and product information.</returns>
    AssetMetadata GetMetadata();
}
