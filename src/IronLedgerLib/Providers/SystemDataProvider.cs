using Microsoft.Management.Infrastructure;

namespace Tudormobile.IronLedgerLib.Providers;

/// <summary>
/// Provides system component data by querying WMI Win32_ComputerSystem.
/// </summary>
internal class SystemDataProvider : CimDataProviderBase
{
    /// <inheritdoc/>
    protected override string WmiClassName => "Win32_ComputerSystem";

    /// <inheritdoc/>
    protected override string CaptionProperty => "Caption";

    /// <inheritdoc/>
    protected override bool HasSerialNumber => false;

    /// <inheritdoc/>
    protected override string GetProductPropertyName(CimInstance instance) => "Model";

    // Easy to maintain: just add or remove property names from this list
    /// <inheritdoc/>
    protected override string[] ComponentPropertyNames =>
    [
        "Description",
        "Model",
        "SystemType",
        "TotalPhysicalMemory",
    ];
}
