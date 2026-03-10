namespace Tudormobile.IronLedgerLib.Providers;

/// <summary>
/// Provides system component data by querying WMI Win32_ComputerSystem.
/// </summary>
internal class SystemDataProvider : CimDataProviderBase
{
    protected override string WmiClassName => "Win32_ComputerSystem";

    protected override string CaptionProperty => "Caption";

    protected override bool HasSerialNumber => false;

    // Easy to maintain: just add or remove property names from this list
    protected override string[] ComponentPropertyNames =>
    [
        "Description",
        "Model",
        "SystemType",
        "TotalPhysicalMemory",
    ];
}
