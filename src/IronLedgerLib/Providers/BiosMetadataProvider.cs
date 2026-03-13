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

        // Join non-empty parts with a space to avoid malformed strings like "NameVersion"
        var productParts = new[] { name, version }
            .Where(s => !string.IsNullOrEmpty(s));
        var product = string.Join(" ", productParts);

        return (serialNumber, manufacturer, product);
    }
}
