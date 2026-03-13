namespace Tudormobile.IronLedgerLib.Providers;

/// <summary>
/// Provides physical memory component data by querying WMI Win32_PhysicalMemory.
/// </summary>
internal class MemoryDataProvider : CimDataProviderBase
{
    /// <inheritdoc/>
    protected override string WmiClassName => "Win32_PhysicalMemory";

    /// <inheritdoc/>
    protected override string CaptionProperty => "Caption";

    // Easy to maintain: just add or remove property names from this list
    /// <inheritdoc/>
    protected override string[] ComponentPropertyNames =>
    [
        "PartNumber",
        "BankLabel",
        "DeviceLocator",
        "Capacity",
        "Speed",
    ];
}