using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tudormobile.IronLedgerLib.Providers;

namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Factory for creating collections of component data from various hardware components.
/// </summary>
public class ComponentDataFactory : IComponentDataFactory
{
    private readonly IComponentDataProvider _processorProvider;
    private readonly IComponentDataProvider _systemProvider;
    private readonly IComponentDataProvider _memoryProvider;
    private readonly IComponentDataProvider _diskProvider;
    private readonly ILogger<ComponentDataFactory> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDataFactory"/> class.
    /// </summary>
    /// <param name="processorProvider">The provider for processor component data. If <see langword="null"/>, uses the default Windows WMI provider.</param>
    /// <param name="systemProvider">The provider for system component data. If <see langword="null"/>, uses the default Windows WMI provider.</param>
    /// <param name="memoryProvider">The provider for memory component data. If <see langword="null"/>, uses the default Windows WMI provider.</param>
    /// <param name="diskProvider">The provider for disk component data. If <see langword="null"/>, uses the default Windows WMI provider.</param>
    /// <param name="logger">The logger for diagnostic and error messages. If <see langword="null"/>, logging is suppressed.</param>
    public ComponentDataFactory(
        IComponentDataProvider? processorProvider = null,
        IComponentDataProvider? systemProvider = null,
        IComponentDataProvider? memoryProvider = null,
        IComponentDataProvider? diskProvider = null,
        ILogger<ComponentDataFactory>? logger = null)
    {
        _processorProvider = processorProvider ?? new ProcessorDataProvider();
        _systemProvider = systemProvider ?? new SystemDataProvider();
        _memoryProvider = memoryProvider ?? new MemoryDataProvider();
        _diskProvider = diskProvider ?? new DiskDataProvider();
        _logger = logger ?? NullLogger<ComponentDataFactory>.Instance;
    }

    /// <summary>
    /// Creates a new <see cref="SystemComponentData"/> by collecting data from all configured providers.
    /// </summary>
    /// <returns>A new <see cref="SystemComponentData"/> instance populated with data from all hardware components.</returns>
    /// <exception cref="ComponentDataProviderException">Thrown when any of the configured providers fail to retrieve data.</exception>
    public SystemComponentData Create()
    {
        _logger.LogDebug("Creating system component data from all providers.");
        try
        {
            var systems = _systemProvider.GetData();
            var processors = _processorProvider.GetData();
            var memory = _memoryProvider.GetData();
            var disks = _diskProvider.GetData();

            _logger.LogInformation(
                "System component data created: {ProcessorCount} processor(s), {MemoryCount} memory module(s), {DiskCount} disk(s).",
                processors.Count, memory.Count, disks.Count);

            return new SystemComponentData
            {
                System = systems.Count > 0 ? systems[0] : ComponentData.Empty,
                Processors = processors,
                Memory = memory,
                Disks = disks
            };
        }
        catch (ComponentDataProviderException ex)
        {
            _logger.LogError(ex, "Provider '{ProviderName}' failed during system component data creation.", ex.ProviderName);
            throw;
        }
    }

    /// <summary>
    /// Gets processor component data.
    /// </summary>
    /// <returns>A collection of processor component data.</returns>
    /// <exception cref="ComponentDataProviderException">Thrown when the processor provider fails to retrieve data.</exception>
    public IReadOnlyList<ComponentData> GetProcessors()
    {
        _logger.LogDebug("Retrieving processor component data.");
        try
        {
            var data = _processorProvider.GetData();
            _logger.LogDebug("Retrieved {Count} processor(s).", data.Count);
            return data;
        }
        catch (ComponentDataProviderException ex)
        {
            _logger.LogError(ex, "Provider '{ProviderName}' failed to retrieve processor data.", ex.ProviderName);
            throw;
        }
    }

    /// <summary>
    /// Gets system component data.
    /// </summary>
    /// <returns>System component data.</returns>
    /// <exception cref="ComponentDataProviderException">Thrown when the system provider fails to retrieve data.</exception>
    public ComponentData GetSystem()
    {
        _logger.LogDebug("Retrieving system component data.");
        try
        {
            var data = _systemProvider.GetData();
            var result = data.Count > 0 ? data[0] : ComponentData.Empty;
            _logger.LogDebug("Retrieved system component data (Caption: '{Caption}').", result.Caption);
            return result;
        }
        catch (ComponentDataProviderException ex)
        {
            _logger.LogError(ex, "Provider '{ProviderName}' failed to retrieve system data.", ex.ProviderName);
            throw;
        }
    }

    /// <summary>
    /// Gets memory component data.
    /// </summary>
    /// <returns>A collection of memory component data.</returns>
    /// <exception cref="ComponentDataProviderException">Thrown when the memory provider fails to retrieve data.</exception>
    public IReadOnlyList<ComponentData> GetMemory()
    {
        _logger.LogDebug("Retrieving memory component data.");
        try
        {
            var data = _memoryProvider.GetData();
            _logger.LogDebug("Retrieved {Count} memory module(s).", data.Count);
            return data;
        }
        catch (ComponentDataProviderException ex)
        {
            _logger.LogError(ex, "Provider '{ProviderName}' failed to retrieve memory data.", ex.ProviderName);
            throw;
        }
    }

    /// <summary>
    /// Gets disk component data.
    /// </summary>
    /// <returns>A collection of disk component data.</returns>
    /// <exception cref="ComponentDataProviderException">Thrown when the disk provider fails to retrieve data.</exception>
    public IReadOnlyList<ComponentData> GetDisks()
    {
        _logger.LogDebug("Retrieving disk component data.");
        try
        {
            var data = _diskProvider.GetData();
            _logger.LogDebug("Retrieved {Count} disk(s).", data.Count);
            return data;
        }
        catch (ComponentDataProviderException ex)
        {
            _logger.LogError(ex, "Provider '{ProviderName}' failed to retrieve disk data.", ex.ProviderName);
            throw;
        }
    }
}
