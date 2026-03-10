namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Defines a provider that retrieves component data including metadata and properties.
/// </summary>
public interface IComponentDataProvider
{
    /// <summary>
    /// Retrieves the component data from hardware components.
    /// </summary>
    /// <returns>A collection of component data instances.</returns>
    IReadOnlyList<ComponentData> GetData();
}
