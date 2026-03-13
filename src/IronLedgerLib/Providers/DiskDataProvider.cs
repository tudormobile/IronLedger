namespace Tudormobile.IronLedgerLib.Providers;

/// <summary>
/// Provides physical disk component data by querying WMI Win32_DiskDrive.
/// </summary>
internal class DiskDataProvider : CimDataProviderBase
{
    /// <inheritdoc/>
    protected override string WmiClassName => "Win32_DiskDrive";

    /// <inheritdoc/>
    protected override string CaptionProperty => "Caption";

    // Easy to maintain: just add or remove property names from this list
    /// <inheritdoc/>
    protected override string[] ComponentPropertyNames =>
    [
        "Description",
        "DeviceID",
        "Model",
        "FirmwareRevision",
        "Size",
        "MediaType",
        "InterfaceType",
        "Partitions"
    ];
}
