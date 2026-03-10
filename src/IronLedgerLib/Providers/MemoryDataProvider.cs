namespace Tudormobile.IronLedgerLib.Providers;

/// <summary>
/// Provides physical memory component data by querying WMI Win32_PhysicalMemory.
/// </summary>
internal class MemoryDataProvider : CimDataProviderBase
{
    protected override string WmiClassName => "Win32_PhysicalMemory";

    protected override string CaptionProperty => "Caption";

    // Easy to maintain: just add or remove property names from this list
    protected override string[] ComponentPropertyNames =>
    [
        "PartNumber",
        "BankLabel",
        "DeviceLocator",
        "Capacity",
        "Speed",
    ];
}
/* Usage Example:
var memoryProvider = new MemoryDataProvider();
var memoryModules = memoryProvider.GetData();

foreach (var module in memoryModules)
{
    Console.WriteLine($"Caption: {module.Caption}");
    foreach (var prop in module.Properties)
    {
        Console.WriteLine($"  {prop.Name}: {prop.Value}");
    }
}
*/