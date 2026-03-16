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

    /// <inheritdoc/>
    public string ContentType => "application/json";

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
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        return JsonSerializer.Serialize(value, _options);
    }

    /// <inheritdoc/>
    public T? Deserialize<T>(string data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(data, nameof(data));
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
