namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Defines a serializer for IronLedger types.
/// </summary>
/// <remarks>
/// Implementations can use any serialization format (JSON, XML, binary, etc.).
/// The default implementation uses System.Text.Json.
/// </remarks>
public interface IIronLedgerSerializer
{
    /// <summary>
    /// Serializes an object to a string representation.
    /// </summary>
    /// <typeparam name="T">The type of object to serialize.</typeparam>
    /// <param name="value">The value to serialize.</param>
    /// <returns>A string representation of the object.</returns>
    string Serialize<T>(T value);

    /// <summary>
    /// Deserializes a string representation to an object.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize.</typeparam>
    /// <param name="data">The string representation to deserialize.</param>
    /// <returns>
    /// The deserialized object. Some implementations may return null if deserialization fails,
    /// while others may throw an exception (for example, when the input is invalid for the
    /// chosen serialization format).
    /// </returns>
    T? Deserialize<T>(string data);
}
