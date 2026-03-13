namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Component data common to all components.
/// </summary>
public record ComponentData
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

    /// <summary>
    /// Determines whether this instance is equal to another <see cref="ComponentData"/> instance.
    /// </summary>
    /// <param name="other">The instance to compare with.</param>
    /// <returns>
    /// <see langword="true"/> if <see cref="Metadata"/>, <see cref="Caption"/>, and the sequence
    /// of <see cref="Properties"/> are all equal; otherwise <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// This override is required because <see cref="IReadOnlyList{T}"/> uses reference equality
    /// by default. Without it, two records with identical but distinct property list instances
    /// would incorrectly compare as unequal.
    /// </remarks>
    public virtual bool Equals(ComponentData? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        return Metadata == other.Metadata
            && Caption == other.Caption
            && Properties.SequenceEqual(other.Properties);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Combines <see cref="Metadata"/>, <see cref="Caption"/>, and each element of
    /// <see cref="Properties"/> so the hash is consistent with <see cref="Equals(ComponentData?)"/>.
    /// </remarks>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Metadata);
        hash.Add(Caption);
        foreach (var prop in Properties)
            hash.Add(prop);
        return hash.ToHashCode();
    }
}
