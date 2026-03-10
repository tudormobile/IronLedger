using Microsoft.Management.Infrastructure;

namespace Tudormobile.IronLedgerLib.Providers;

/// <summary>
/// Provides BIOS metadata by querying WMI Win32_BIOS.
/// </summary>
internal class BiosMetadataProvider : CimMetadataProviderBase
{
    /// <inheritdoc/>
    protected override string WmiClassName => "Win32_BIOS";

    /// <inheritdoc/>
    protected override string Properties => "SerialNumber, Manufacturer, Version, Name";

    /// <inheritdoc/>
    protected override (string? SerialNumber, string? Manufacturer, string? Product) ExtractMetadata(CimInstance instance)
    {
        var serialNumber = GetPropertyValue(instance, "SerialNumber");
        var manufacturer = GetPropertyValue(instance, "Manufacturer");
        var version = GetPropertyValue(instance, "Version");
        var name = GetPropertyValue(instance, "Name");

        return (serialNumber, manufacturer, $"{name}{version}");
    }
}
