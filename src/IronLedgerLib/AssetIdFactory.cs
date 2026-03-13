using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tudormobile.IronLedgerLib.Providers;

namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Factory for creating <see cref="AssetId"/> instances by collecting metadata from multiple hardware components.
/// </summary>
public class AssetIdFactory : IAssetIdFactory
{
    private readonly IAssetMetadataProvider _systemProvider;
    private readonly IAssetMetadataProvider _baseboardProvider;
    private readonly IAssetMetadataProvider _biosProvider;
    private readonly ILogger<AssetIdFactory> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssetIdFactory"/> class.
    /// </summary>
    /// <param name="systemProvider">The provider for system-level metadata. If <see langword="null"/>, uses the default Windows WMI provider.</param>
    /// <param name="baseboardProvider">The provider for baseboard metadata. If <see langword="null"/>, uses the default Windows WMI provider.</param>
    /// <param name="biosProvider">The provider for BIOS metadata. If <see langword="null"/>, uses the default Windows WMI provider.</param>
    /// <param name="logger">The logger for diagnostic and error messages. If <see langword="null"/>, logging is suppressed.</param>
    public AssetIdFactory(
        IAssetMetadataProvider? systemProvider = null,
        IAssetMetadataProvider? baseboardProvider = null,
        IAssetMetadataProvider? biosProvider = null,
        ILogger<AssetIdFactory>? logger = null)
    {
        _systemProvider = systemProvider ?? new SystemMetadataProvider();
        _baseboardProvider = baseboardProvider ?? new BaseboardMetadataProvider();
        _biosProvider = biosProvider ?? new BiosMetadataProvider();
        _logger = logger ?? NullLogger<AssetIdFactory>.Instance;
    }

    /// <summary>
    /// Creates a new <see cref="AssetId"/> by collecting metadata from all configured providers.
    /// </summary>
    /// <returns>A new <see cref="AssetId"/> instance populated with metadata from system, baseboard, and BIOS.</returns>
    /// <exception cref="ComponentDataProviderException">Thrown when any of the configured metadata providers fail to retrieve data.</exception>
    public AssetId Create()
    {
        _logger.LogDebug("Creating asset ID from system, baseboard, and BIOS metadata.");
        try
        {
            var system = _systemProvider.GetMetadata();
            var baseboard = _baseboardProvider.GetMetadata();
            var bios = _biosProvider.GetMetadata();

            _logger.LogDebug("Asset ID metadata retrieved successfully.");

            return new AssetId
            {
                SystemMetadata = system,
                BaseBoardMetadata = baseboard,
                BiosMetadata = bios
            };
        }
        catch (ComponentDataProviderException ex)
        {
            _logger.LogError(ex, "Provider '{ProviderName}' failed to retrieve asset metadata.", ex.ProviderName);
            throw;
        }
    }
}
