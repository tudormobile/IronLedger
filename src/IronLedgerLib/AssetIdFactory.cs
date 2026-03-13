using Tudormobile.IronLedgerLib.Providers;

namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Factory for creating <see cref="AssetId"/> instances by collecting metadata from multiple hardware components.
/// </summary>
public class AssetIdFactory
{
    private readonly IAssetMetadataProvider _systemProvider;
    private readonly IAssetMetadataProvider _baseboardProvider;
    private readonly IAssetMetadataProvider _biosProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssetIdFactory"/> class.
    /// </summary>
    /// <param name="systemProvider">The provider for system-level metadata. If null, uses the default Windows WMI provider.</param>
    /// <param name="baseboardProvider">The provider for baseboard metadata. If null, uses the default Windows WMI provider.</param>
    /// <param name="biosProvider">The provider for BIOS metadata. If null, uses the default Windows WMI provider.</param>
    public AssetIdFactory(
        IAssetMetadataProvider? systemProvider = null,
        IAssetMetadataProvider? baseboardProvider = null,
        IAssetMetadataProvider? biosProvider = null)
    {
        _systemProvider = systemProvider ?? new SystemMetadataProvider();
        _baseboardProvider = baseboardProvider ?? new BaseboardMetadataProvider();
        _biosProvider = biosProvider ?? new BiosMetadataProvider();
    }

    /// <summary>
    /// Creates a new <see cref="AssetId"/> by collecting metadata from all configured providers.
    /// </summary>
    /// <returns>A new <see cref="AssetId"/> instance populated with metadata from system, baseboard, and BIOS.</returns>
    /// <exception cref="ComponentDataProviderException">Thrown when any of the configured metadata providers fail to retrieve data.</exception>
    public AssetId Create()
    {
        return new AssetId
        {
            SystemMetadata = _systemProvider.GetMetadata(),
            BaseBoardMetadata = _baseboardProvider.GetMetadata(),
            BiosMetadata = _biosProvider.GetMetadata()
        };
    }
}
