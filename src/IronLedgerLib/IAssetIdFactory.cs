namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Defines a factory for creating <see cref="AssetId"/> instances.
/// </summary>
public interface IAssetIdFactory
{
    /// <summary>
    /// Creates a new <see cref="AssetId"/> by collecting metadata from all configured hardware components.
    /// </summary>
    /// <returns>A new <see cref="AssetId"/> instance populated with metadata from system, baseboard, and BIOS.</returns>
    /// <exception cref="ComponentDataProviderException">Thrown when any of the configured metadata providers fail to retrieve data.</exception>
    AssetId Create();
}
