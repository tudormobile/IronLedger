using Microsoft.Management.Infrastructure;

namespace Tudormobile.IronLedgerLib.Providers;

/// <summary>
/// Base class for metadata providers that query WMI using CIM.
/// </summary>
internal abstract class CimMetadataProviderBase : IAssetMetadataProvider
{
    /// <summary>
    /// Gets the WMI class name to query.
    /// </summary>
    protected abstract string WmiClassName { get; }

    /// <summary>
    /// Gets the comma-separated list of properties to select from the WMI class.
    /// </summary>
    protected abstract string Properties { get; }

    /// <inheritdoc/>
    public AssetMetadata GetMetadata()
    {
        using var session = CimSession.Create(null);
        var query = $"SELECT {Properties} FROM {WmiClassName}";
        var instances = session.QueryInstances(@"root\cimv2", "WQL", query);

        foreach (var instance in instances)
        {
            var (serialNumber, manufacturer, product) = ExtractMetadata(instance);
            return new AssetMetadata
            {
                SerialNumber = serialNumber ?? string.Empty,
                Manufacturer = manufacturer ?? string.Empty,
                Product = product ?? string.Empty
            };
        }
        return AssetMetadata.Empty;
    }

    /// <summary>
    /// Extracts metadata from a CIM instance as a tuple.
    /// </summary>
    /// <param name="instance">The CIM instance containing the property values.</param>
    /// <returns>A tuple containing (SerialNumber, Manufacturer, Product).</returns>
    protected abstract (string? SerialNumber, string? Manufacturer, string? Product) ExtractMetadata(CimInstance instance);

    /// <summary>
    /// Gets a property value from a CIM instance as a string.
    /// </summary>
    /// <param name="instance">The CIM instance.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>The property value as a string, or null if not found or empty.</returns>
    protected static string? GetPropertyValue(CimInstance instance, string propertyName)
        => instance.CimInstanceProperties[propertyName]?.Value?.ToString();
}
