using Microsoft.Management.Infrastructure;

namespace Tudormobile.IronLedgerLib.Providers;

/// <summary>
/// Provides system-level metadata by querying WMI Win32_ComputerSystem.
/// </summary>
internal class SystemMetadataProvider : CimMetadataProviderBase
{
    /// <inheritdoc/>
    protected override string WmiClassName => "Win32_ComputerSystem";

    /// <inheritdoc/>
    protected override string Properties => "Manufacturer, Model";

    /// <inheritdoc/>
    protected override (string? SerialNumber, string? Manufacturer, string? Product) ExtractMetadata(CimInstance instance)
    {
        var manufacturer = GetPropertyValue(instance, "Manufacturer");
        var model = GetPropertyValue(instance, "Model");

        return (null, manufacturer, model);
    }
}