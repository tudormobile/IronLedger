using Microsoft.Management.Infrastructure;

namespace Tudormobile.IronLedgerLib.Providers;

/// <summary>
/// Provides baseboard (motherboard) metadata by querying WMI Win32_BaseBoard.
/// </summary>
internal class BaseboardMetadataProvider : CimMetadataProviderBase
{
    /// <inheritdoc/>
    protected override string WmiClassName => "Win32_BaseBoard";

    /// <inheritdoc/>
    protected override string Properties => "SerialNumber, Manufacturer, Product";

    /// <inheritdoc/>
    protected override (string? SerialNumber, string? Manufacturer, string? Product) ExtractMetadata(CimInstance instance)
    {
        var serialNumber = GetPropertyValue(instance, "SerialNumber");
        var manufacturer = GetPropertyValue(instance, "Manufacturer");
        var product = GetPropertyValue(instance, "Product");

        return (serialNumber, manufacturer, product);
    }
}