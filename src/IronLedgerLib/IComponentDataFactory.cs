namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Defines a factory for creating component data from hardware components.
/// </summary>
public interface IComponentDataFactory
{
    /// <summary>
    /// Creates a new <see cref="SystemComponentData"/> by collecting data from all configured providers.
    /// </summary>
    /// <returns>A new <see cref="SystemComponentData"/> instance populated with data from all hardware components.</returns>
    /// <exception cref="ComponentDataProviderException">Thrown when any of the configured providers fail to retrieve data.</exception>
    SystemComponentData Create();

    /// <summary>
    /// Gets processor component data.
    /// </summary>
    /// <returns>A collection of processor component data.</returns>
    /// <exception cref="ComponentDataProviderException">Thrown when the processor provider fails to retrieve data.</exception>
    IReadOnlyList<ComponentData> GetProcessors();

    /// <summary>
    /// Gets system component data.
    /// </summary>
    /// <returns>System component data.</returns>
    /// <exception cref="ComponentDataProviderException">Thrown when the system provider fails to retrieve data.</exception>
    ComponentData GetSystem();

    /// <summary>
    /// Gets memory component data.
    /// </summary>
    /// <returns>A collection of memory component data.</returns>
    /// <exception cref="ComponentDataProviderException">Thrown when the memory provider fails to retrieve data.</exception>
    IReadOnlyList<ComponentData> GetMemory();

    /// <summary>
    /// Gets disk component data.
    /// </summary>
    /// <returns>A collection of disk component data.</returns>
    /// <exception cref="ComponentDataProviderException">Thrown when the disk provider fails to retrieve data.</exception>
    IReadOnlyList<ComponentData> GetDisks();
}
