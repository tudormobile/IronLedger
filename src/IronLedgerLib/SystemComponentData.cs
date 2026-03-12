namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Represents an aggregated collection of all system component data.
/// </summary>
public record SystemComponentData
{
    /// <summary>
    /// Gets the system components.
    /// </summary>
    public required ComponentData System { get; init; }

    /// <summary>
    /// Gets the collection of processor components.
    /// </summary>
    public required IReadOnlyList<ComponentData> Processors { get; init; }

    /// <summary>
    /// Gets the collection of memory components.
    /// </summary>
    public required IReadOnlyList<ComponentData> Memory { get; init; }

    /// <summary>
    /// Gets the collection of disk components.
    /// </summary>
    public required IReadOnlyList<ComponentData> Disks { get; init; }
}
