namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Represents an asset identifier that can use different identification types.
/// </summary>
public record AssetId
{
    private string? _idCache;

    /// <summary>
    /// Gets the system level metadata associated with this asset.
    /// </summary>
    public required AssetMetadata SystemMetadata { get; init; }

    /// <summary>
    /// Gets the baseboard (motherboard) level metadata associated with this asset.
    /// </summary>
    public required AssetMetadata BaseBoardMetadata { get; init; }

    /// <summary>
    /// Gets the bios level metadata associated with this asset.
    /// </summary>
    public required AssetMetadata BiosMetadata { get; init; }

    /// <summary>
    /// Gets a unique identifier derived from the combined metadata.
    /// This ID is deterministic and suitable for use as a database key.
    /// </summary>
    /// <remarks>
    /// The ID is generated using SHA256 hash of all metadata properties,
    /// ensuring consistency across multiple instantiations with the same data.
    /// The hash is computed lazily on first access and cached for subsequent calls.
    /// The cache is intentionally excluded from <see langword="with"/> expression copies
    /// so that modified instances always compute a fresh ID from their own metadata.
    /// </remarks>
    public string Id => _idCache ??= ComputeId();

    /// <summary>
    /// Copy constructor invoked by <see langword="with"/> expressions.
    /// Intentionally excludes <c>_idCache</c> so that modified copies always
    /// compute a fresh identifier from their own metadata.
    /// </summary>
    /// <param name="original">The original instance to copy properties from.</param>
    protected AssetId(AssetId original)
    {
        SystemMetadata = original.SystemMetadata;
        BaseBoardMetadata = original.BaseBoardMetadata;
        BiosMetadata = original.BiosMetadata;
    }

    /// <summary>
    /// Returns the unique identifier for this asset.
    /// </summary>
    /// <returns>A 64-character hexadecimal string representing the unique identifier.</returns>
    public override string ToString() => Id;

    /// <summary>
    /// Determines whether the specified string is a valid asset identifier.
    /// A valid identifier is a 64-character lowercase hexadecimal string produced by a SHA-256 hash.
    /// </summary>
    /// <param name="id">The string to validate.</param>
    /// <returns><see langword="true"/> if <paramref name="id"/> is a valid asset identifier; otherwise, <see langword="false"/>.</returns>
    public static bool IsValid(string? id)
        => id is { Length: 64 } && id.All(c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f'));

    private string ComputeId()
    {
        // Combine all metadata into a single string using pipe delimiter
        var input = string.Join("|",
            SystemMetadata.SerialNumber,
            SystemMetadata.Manufacturer,
            SystemMetadata.Product,
            BaseBoardMetadata.SerialNumber,
            BaseBoardMetadata.Manufacturer,
            BaseBoardMetadata.Product,
            BiosMetadata.SerialNumber,
            BiosMetadata.Manufacturer,
            BiosMetadata.Product
        );

        var hashBytes = System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(input));
        return Convert.ToHexStringLower(hashBytes);
    }
}
