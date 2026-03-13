using Microsoft.Management.Infrastructure;
using System.Text;

namespace Tudormobile.IronLedgerLib.Providers;

/// <summary>
/// Base class for component data providers that query WMI using CIM.
/// </summary>
internal abstract class CimDataProviderBase : IComponentDataProvider
{
    /// <summary>
    /// Gets the WMI class name to query.
    /// </summary>
    protected abstract string WmiClassName { get; }

    /// <summary>
    /// Gets the property name used for the component caption.
    /// </summary>
    protected abstract string CaptionProperty { get; }

    /// <summary>
    /// Gets the list of CIM property names to include in the component properties collection.
    /// These properties will be extracted and added to the ComponentProperty list in the order specified.
    /// </summary>
    protected abstract string[] ComponentPropertyNames { get; }

    /// <summary>
    /// Gets a value indicating whether the object is expected to have a serial number associated with it.
    /// </summary>
    /// <remarks>
    /// The default implementation returns <see langword="true"/>. Override this property in a derived class
    /// and return <see langword="false"/> when serial numbers are not applicable for the WMI class being queried
    /// (for example, <c>Win32_ComputerSystem</c> does not expose a serial number).
    /// </remarks>
    protected virtual bool HasSerialNumber => true;

    /// <inheritdoc/>
    /// <exception cref="ComponentDataProviderException">Thrown when WMI/CIM query fails or data retrieval encounters an error.</exception>
    public IReadOnlyList<ComponentData> GetData()
    {
        try
        {
            using var session = CimSession.Create(null);

            // Build the property list for the SELECT query
            var metadataProps = HasSerialNumber
                ? ["SerialNumber", "Manufacturer"]
                : new[] { "Manufacturer" };
            var allProperties = metadataProps
                .Concat(new[] { CaptionProperty })
                .Concat(ComponentPropertyNames)
                .Distinct()
                .ToArray();

            var query = $"SELECT {string.Join(", ", allProperties)} FROM {WmiClassName}";
            var instances = session.QueryInstances(@"root\cimv2", "WQL", query);

            var results = new List<ComponentData>();
            foreach (var instance in instances)
            {
                var metadata = ExtractMetadata(instance);
                var caption = GetPropertyValue(instance, CaptionProperty) ?? string.Empty;
                var properties = ExtractComponentProperties(instance);

                results.Add(new ComponentData
                {
                    Metadata = metadata,
                    Caption = caption,
                    Properties = properties
                });
            }

            return results;
        }
        catch (Exception ex) when (ex is not ComponentDataProviderException)
        {
            throw new ComponentDataProviderException(
                $"Failed to retrieve data from WMI class '{WmiClassName}'.",
                ex)
            {
                ProviderName = GetType().Name,
                WmiClassName = WmiClassName
            };
        }
    }

    /// <summary>
    /// Extracts metadata from a CIM instance.
    /// Derived classes can override to customize which properties map to SerialNumber, Manufacturer, and Product.
    /// </summary>
    protected virtual AssetMetadata ExtractMetadata(CimInstance instance)
    {
        var serialNumber = HasSerialNumber ? GetPropertyValue(instance, "SerialNumber") : null;
        var manufacturer = GetPropertyValue(instance, "Manufacturer");
        var product = GetPropertyValue(instance, GetProductPropertyName(instance));

        return new AssetMetadata
        {
            SerialNumber = serialNumber ?? string.Empty,
            Manufacturer = manufacturer ?? string.Empty,
            Product = product ?? string.Empty
        };
    }

    /// <summary>
    /// Gets the property name to use for the Product field in metadata.
    /// Defaults to the caption property. Override to use a different property.
    /// </summary>
    protected virtual string GetProductPropertyName(CimInstance instance) => CaptionProperty;

    /// <summary>
    /// Extracts component properties from a CIM instance based on the declared property names.
    /// Override to customize property extraction or display names.
    /// </summary>
    protected virtual IReadOnlyList<ComponentProperty> ExtractComponentProperties(CimInstance instance)
    {
        var list = new List<ComponentProperty>();

        foreach (var propertyName in ComponentPropertyNames)
        {
            var value = GetPropertyValue(instance, propertyName) ?? string.Empty;
            var displayName = FormatPropertyName(propertyName);
            list.Add(new ComponentProperty(displayName, value));
        }

        return list;
    }

    /// <summary>
    /// Formats a CIM property name for display.
    /// Converts camel case to space-separated words (e.g., "DeviceID" -> "Device ID").
    /// Override to provide custom formatting.
    /// </summary>
    protected virtual string FormatPropertyName(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return propertyName;

        var sb = new StringBuilder(propertyName.Length + 10);
        sb.Append(propertyName[0]);

        for (int i = 1; i < propertyName.Length; i++)
        {
            if (char.IsUpper(propertyName[i]) && !char.IsUpper(propertyName[i - 1]))
            {
                sb.Append(' ');
            }
            sb.Append(propertyName[i]);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Gets a property value from a CIM instance as a string.
    /// </summary>
    protected static string? GetPropertyValue(CimInstance instance, string propertyName)
        => instance.CimInstanceProperties[propertyName]?.Value?.ToString();
}
