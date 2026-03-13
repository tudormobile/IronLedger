namespace Tudormobile.IronLedgerLib.Providers;

/// <summary>
/// Provides processor component data by querying WMI Win32_Processor.
/// </summary>
internal class ProcessorDataProvider : CimDataProviderBase
{
    /// <inheritdoc/>
    protected override string WmiClassName => "Win32_Processor";

    /// <inheritdoc/>
    protected override string CaptionProperty => "Name";

    // Easy to maintain: just add or remove property names from this list
    /// <inheritdoc/>
    protected override string[] ComponentPropertyNames =>
    [
        "Description",
        "Version",
        "DeviceID",
        "NumberOfCores",
        "NumberOfLogicalProcessors",
        "MaxClockSpeed",
        "SocketDesignation",
    ];
}
