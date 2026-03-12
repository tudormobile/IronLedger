namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Represents a name/value property pair for component data.
/// </summary>
public record ComponentProperty(string Name, string Value);

/// <summary>
/// Component data common to all components.
/// </summary>
public class ComponentData
{
    /// <summary>
    /// Gets an empty <see cref="ComponentData"/> instance with all properties set to empty strings.
    /// </summary>
    public static ComponentData Empty { get; } = new ComponentData
    {
        Caption = string.Empty,
        Metadata = AssetMetadata.Empty,
        Properties = []
    };

    /// <summary>
    /// Gets the metadata that provides basic information about the asset's properties and characteristics.
    /// </summary>
    /// <remarks>The Metadata property is initialized when the asset is created and is immutable thereafter.
    /// It contains essential information used for asset management and processing.</remarks>
    public required AssetMetadata Metadata { get; init; }

    /// <summary>
    /// Gets the caption that describes the component.
    /// </summary>
    public required string Caption { get; init; }

    /// <summary>
    /// Gets the ordered collection of component-specific properties.
    /// The order is determined by the provider that creates the component.
    /// </summary>
    public required IReadOnlyList<ComponentProperty> Properties { get; init; }
}
