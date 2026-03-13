using Tudormobile.IronLedgerLib.Providers;

namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Factory for creating collections of component data from various hardware components.
/// </summary>
public class ComponentDataFactory
{
    private readonly IComponentDataProvider _processorProvider;
    private readonly IComponentDataProvider _systemProvider;
    private readonly IComponentDataProvider _memoryProvider;
    private readonly IComponentDataProvider _diskProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDataFactory"/> class.
    /// </summary>
    /// <param name="processorProvider">The provider for processor component data. If null, uses the default Windows WMI provider.</param>
    /// <param name="systemProvider">The provider for system component data. If null, uses the default Windows WMI provider.</param>
    /// <param name="memoryProvider">The provider for memory component data. If null, uses the default Windows WMI provider.</param>
    /// <param name="diskProvider">The provider for disk component data. If null, uses the default Windows WMI provider.</param>
    public ComponentDataFactory(
        IComponentDataProvider? processorProvider = null,
        IComponentDataProvider? systemProvider = null,
        IComponentDataProvider? memoryProvider = null,
        IComponentDataProvider? diskProvider = null)
    {
        _processorProvider = processorProvider ?? new ProcessorDataProvider();
        _systemProvider = systemProvider ?? new SystemDataProvider();
        _memoryProvider = memoryProvider ?? new MemoryDataProvider();
        _diskProvider = diskProvider ?? new DiskDataProvider();
    }

    /// <summary>
    /// Creates a new <see cref="SystemComponentData"/> by collecting data from all configured providers.
    /// </summary>
    /// <returns>A new <see cref="SystemComponentData"/> instance populated with data from all hardware components.</returns>
    /// <exception cref="ComponentDataProviderException">Thrown when any of the configured providers fail to retrieve data.</exception>
    public SystemComponentData Create()
    {
        var systems = _systemProvider.GetData();
        return new SystemComponentData
        {
            System = systems.Count > 0 ? systems[0] : ComponentData.Empty,
            Processors = _processorProvider.GetData(),
            Memory = _memoryProvider.GetData(),
            Disks = _diskProvider.GetData()
        };
    }

    /// <summary>
    /// Gets processor component data.
    /// </summary>
    /// <returns>A collection of processor component data.</returns>
    /// <exception cref="ComponentDataProviderException">Thrown when the processor provider fails to retrieve data.</exception>
    public IReadOnlyList<ComponentData> GetProcessors()
        => _processorProvider.GetData();

    /// <summary>
    /// Gets system component data.
    /// </summary>
    /// <returns>System component data.</returns>
    /// <exception cref="ComponentDataProviderException">Thrown when the system provider fails to retrieve data.</exception>
    public ComponentData GetSystem()
    {
        var data = _systemProvider.GetData();
        return data.Count > 0
            ? data[0]
            : ComponentData.Empty;
    }

    /// <summary>
    /// Gets memory component data.
    /// </summary>
    /// <returns>A collection of memory component data.</returns>
    /// <exception cref="ComponentDataProviderException">Thrown when the memory provider fails to retrieve data.</exception>
    public IReadOnlyList<ComponentData> GetMemory()
        => _memoryProvider.GetData();

    /// <summary>
    /// Gets disk component data.
    /// </summary>
    /// <returns>A collection of disk component data.</returns>
    /// <exception cref="ComponentDataProviderException">Thrown when the disk provider fails to retrieve data.</exception>
    public IReadOnlyList<ComponentData> GetDisks()
        => _diskProvider.GetData();
}
