using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tudormobile.IronLedgerLib.Serialization;

/// <summary>
/// Custom JSON converter that serializes ComponentProperty lists as key-value dictionaries
/// instead of objects with "name" and "value" properties.
/// </summary>
public class ComponentPropertyConverter : JsonConverter<IReadOnlyList<ComponentProperty>>
{
    /// <summary>
    /// Reads a JSON object and converts it into a read-only list of component properties.
    /// </summary>
    /// <param name="reader">The reader positioned at the JSON to deserialize. The reader is advanced as the method processes the input.</param>
    /// <param name="typeToConvert">The type of object to convert. This parameter is provided by the serialization infrastructure and is not used
    /// directly.</param>
    /// <param name="options">The serialization options to use when reading the JSON. This parameter is provided by the serialization
    /// infrastructure and is not used directly.</param>
    /// <returns>A read-only list of component properties deserialized from the JSON object, or null if the JSON token is null.</returns>
    /// <exception cref="JsonException">Thrown if the JSON does not start with an object, contains unexpected tokens, or ends unexpectedly.</exception>
    public override IReadOnlyList<ComponentProperty>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token for ComponentProperty list.");

        var properties = new List<ComponentProperty>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return properties;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected PropertyName token.");

            var propertyName = reader.GetString() ?? string.Empty;

            reader.Read();
            var propertyValue = reader.GetString() ?? string.Empty;

            // Convert snake_case back to a more readable format (optional)
            var displayName = ConvertFromSnakeCase(propertyName);
            properties.Add(new ComponentProperty(displayName, propertyValue));
        }

        throw new JsonException("Unexpected end of JSON while reading ComponentProperty list.");
    }

    /// <summary>
    /// Writes a collection of component properties to JSON using snake_case property names.
    /// </summary>
    /// <remarks>Each property in the collection is written as a JSON string property with its name converted
    /// to snake_case. The method writes a single JSON object containing all properties.</remarks>
    /// <param name="writer">The writer to which the JSON output is written. Must not be null.</param>
    /// <param name="value">The collection of component properties to serialize. Each property's name is converted to snake_case in the
    /// output.</param>
    /// <param name="options">Options to control the behavior of the JSON serialization. Can be used to customize serialization settings.</param>
    public override void Write(Utf8JsonWriter writer, IReadOnlyList<ComponentProperty> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var property in value)
        {
            // Convert property name to snake_case to match the naming policy
            var propertyName = ConvertToSnakeCase(property.Name);
            writer.WriteString(propertyName, property.Value);
        }

        writer.WriteEndObject();
    }

    private static string ConvertToSnakeCase(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        // Handle common patterns in hardware property names
        var result = text
            .Replace(" ", "_")          // Spaces to underscores
            .Replace("(", "")           // Remove parentheses
            .Replace(")", "")
            .Replace("/", "_")          // Forward slash to underscore
            .Replace("\\", "_")         // Backslash to underscore
            .Replace("-", "_")          // Hyphen to underscore
            .ToLowerInvariant();        // Convert to lowercase

        // Clean up multiple consecutive underscores
        while (result.Contains("__"))
        {
            result = result.Replace("__", "_");
        }

        return result.Trim('_');
    }

    private static string ConvertFromSnakeCase(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        // Convert snake_case back to Title Case with spaces
        var words = text.Split('_', StringSplitOptions.RemoveEmptyEntries);
        return string.Join(" ", words.Select(w =>
            char.ToUpperInvariant(w[0]) + w.Substring(1)));
    }
}
