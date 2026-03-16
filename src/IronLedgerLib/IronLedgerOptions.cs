namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Configuration options for IronLedger service registration.
/// </summary>
/// <remarks>
/// All properties default to <see langword="null"/>, which causes the corresponding
/// service to use its built-in default Windows WMI provider or serializer.
/// </remarks>
public class IronLedgerOptions
{
    /// <summary>
    /// Gets or sets a custom serializer. If <see langword="null"/>, the default
    /// <see cref="Serialization.IronLedgerJsonSerializer"/> is used.
    /// </summary>
    public IIronLedgerSerializer? Serializer { get; set; }

    /// <summary>
    /// Gets or sets a custom processor data provider for <see cref="IComponentDataFactory"/>.
    /// If <see langword="null"/>, the default Windows WMI provider is used.
    /// </summary>
    public IComponentDataProvider? ProcessorProvider { get; set; }

    /// <summary>
    /// Gets or sets a custom system data provider for <see cref="IComponentDataFactory"/>.
    /// If <see langword="null"/>, the default Windows WMI provider is used.
    /// </summary>
    public IComponentDataProvider? SystemProvider { get; set; }

    /// <summary>
    /// Gets or sets a custom memory data provider for <see cref="IComponentDataFactory"/>.
    /// If <see langword="null"/>, the default Windows WMI provider is used.
    /// </summary>
    public IComponentDataProvider? MemoryProvider { get; set; }

    /// <summary>
    /// Gets or sets a custom disk data provider for <see cref="IComponentDataFactory"/>.
    /// If <see langword="null"/>, the default Windows WMI provider is used.
    /// </summary>
    public IComponentDataProvider? DiskProvider { get; set; }

    /// <summary>
    /// Gets or sets a custom system-level metadata provider for <see cref="IAssetIdFactory"/>.
    /// If <see langword="null"/>, the default Windows WMI provider is used.
    /// </summary>
    public IAssetMetadataProvider? SystemMetadataProvider { get; set; }

    /// <summary>
    /// Gets or sets a custom baseboard metadata provider for <see cref="IAssetIdFactory"/>.
    /// If <see langword="null"/>, the default Windows WMI provider is used.
    /// </summary>
    public IAssetMetadataProvider? BaseboardMetadataProvider { get; set; }

    /// <summary>
    /// Gets or sets a custom BIOS metadata provider for <see cref="IAssetIdFactory"/>.
    /// If <see langword="null"/>, the default Windows WMI provider is used.
    /// </summary>
    public IAssetMetadataProvider? BiosMetadataProvider { get; set; }

    /// <summary>
    /// Gets or sets the root directory used by the file-system asset repository.
    /// If <see langword="null"/>, defaults to a subdirectory named <c>data</c> inside
    /// the current working directory.
    /// </summary>
    public string? DataPath { get; set; }
}
