namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Represents an asset identifier that can use different identification types.
/// </summary>
public record AssetId
{
    private Lazy<string>? _id;

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
    /// </remarks>
    public string Id
    {
        get
        {
            _id ??= new Lazy<string>(() =>
            {
                using var sha256 = System.Security.Cryptography.SHA256.Create();

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

                var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                return Convert.ToHexStringLower(hashBytes);
            });

            return _id.Value;
        }
    }

    /// <summary>
    /// Returns the unique identifier for this asset.
    /// </summary>
    /// <returns>A 64-character hexadecimal string representing the unique identifier.</returns>
    public override string ToString() => Id;
}
