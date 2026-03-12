using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tudormobile.IronLedgerLib.Serialization;

/// <summary>
/// Default JSON-based implementation of <see cref="IIronLedgerSerializer"/>.
/// </summary>
/// <remarks>
/// Uses System.Text.Json with configured options for consistent serialization
/// of IronLedger types.
/// </remarks>
public class IronLedgerJsonSerializer : IIronLedgerSerializer
{
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="IronLedgerJsonSerializer"/> class
    /// with default options.
    /// </summary>
    public IronLedgerJsonSerializer()
        : this(CreateDefaultOptions())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IronLedgerJsonSerializer"/> class
    /// with custom options.
    /// </summary>
    /// <param name="options">Custom JSON serializer options.</param>
    public IronLedgerJsonSerializer(JsonSerializerOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc/>
    public string Serialize<T>(T value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        return JsonSerializer.Serialize(value, _options);
    }

    /// <inheritdoc/>
    public T? Deserialize<T>(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            throw new ArgumentException("Data cannot be null or whitespace.", nameof(data));

        return JsonSerializer.Deserialize<T>(data, _options);
    }

    /// <summary>
    /// Creates default JSON serializer options configured for IronLedger types.
    /// </summary>
    /// <returns>Configured <see cref="JsonSerializerOptions"/>.</returns>
    public static JsonSerializerOptions CreateDefaultOptions()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new ComponentPropertyConverter()
            }
        };

        return options;
    }
}
