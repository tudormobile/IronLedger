namespace Tudormobile.IronLedgerLib.Providers;

/// <summary>
/// Provides processor component data by querying WMI Win32_Processor.
/// </summary>
internal class ProcessorDataProvider : CimDataProviderBase
{
    protected override string WmiClassName => "Win32_Processor";

    protected override string CaptionProperty => "Name";

    // Easy to maintain: just add or remove property names from this list
    protected override string[] ComponentPropertyNames =>
    [
        "Name",
        "Description",
        "Version",
        "DeviceID",
        "NumberOfCores",
        "NumberOfLogicalProcessors",
        "MaxClockSpeed",
        "SocketDesignation",
    ];
}
