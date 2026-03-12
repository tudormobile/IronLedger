namespace Tudormobile.IronLedgerLib;

/// <summary>
/// Extension methods for serialization of IronLedger types.
/// </summary>
public static class SerializationExtensions
{
    /// <summary>
    /// Serializes an <see cref="AssetId"/> to a string using the specified serializer.
    /// </summary>
    /// <param name="assetId">The asset ID to serialize.</param>
    /// <param name="serializer">The serializer to use. If null, uses the default JSON serializer.</param>
    /// <returns>A string representation of the asset ID.</returns>
    public static string Serialize(this AssetId assetId, IIronLedgerSerializer? serializer = null)
    {
        serializer ??= new Serialization.IronLedgerJsonSerializer();
        return serializer.Serialize(assetId);
    }

    /// <summary>
    /// Serializes a <see cref="ComponentData"/> to a string using the specified serializer.
    /// </summary>
    /// <param name="componentData">The component data to serialize.</param>
    /// <param name="serializer">The serializer to use. If null, uses the default JSON serializer.</param>
    /// <returns>A string representation of the component data.</returns>
    public static string Serialize(this ComponentData componentData, IIronLedgerSerializer? serializer = null)
    {
        serializer ??= new Serialization.IronLedgerJsonSerializer();
        return serializer.Serialize(componentData);
    }

    /// <summary>
    /// Serializes a <see cref="SystemComponentData"/> to a string using the specified serializer.
    /// </summary>
    /// <param name="systemComponentData">The system component data to serialize.</param>
    /// <param name="serializer">The serializer to use. If null, uses the default JSON serializer.</param>
    /// <returns>A string representation of the system component data.</returns>
    public static string Serialize(this SystemComponentData systemComponentData, IIronLedgerSerializer? serializer = null)
    {
        serializer ??= new Serialization.IronLedgerJsonSerializer();
        return serializer.Serialize(systemComponentData);
    }

    /// <summary>
    /// Deserializes a string to an <see cref="AssetId"/> using the specified serializer.
    /// </summary>
    /// <param name="data">The string representation to deserialize.</param>
    /// <param name="serializer">The serializer to use. If null, uses the default JSON serializer.</param>
    /// <returns>The deserialized asset ID, or null if deserialization fails.</returns>
    public static AssetId? DeserializeAssetId(this string data, IIronLedgerSerializer? serializer = null)
    {
        serializer ??= new Serialization.IronLedgerJsonSerializer();
        return serializer.Deserialize<AssetId>(data);
    }

    /// <summary>
    /// Deserializes a string to a <see cref="ComponentData"/> using the specified serializer.
    /// </summary>
    /// <param name="data">The string representation to deserialize.</param>
    /// <param name="serializer">The serializer to use. If null, uses the default JSON serializer.</param>
    /// <returns>The deserialized component data, or null if deserialization fails.</returns>
    public static ComponentData? DeserializeComponentData(this string data, IIronLedgerSerializer? serializer = null)
    {
        serializer ??= new Serialization.IronLedgerJsonSerializer();
        return serializer.Deserialize<ComponentData>(data);
    }

    /// <summary>
    /// Deserializes a string to a <see cref="SystemComponentData"/> using the specified serializer.
    /// </summary>
    /// <param name="data">The string representation to deserialize.</param>
    /// <param name="serializer">The serializer to use. If null, uses the default JSON serializer.</param>
    /// <returns>The deserialized system component data, or null if deserialization fails.</returns>
    public static SystemComponentData? DeserializeSystemComponentData(this string data, IIronLedgerSerializer? serializer = null)
    {
        serializer ??= new Serialization.IronLedgerJsonSerializer();
        return serializer.Deserialize<SystemComponentData>(data);
    }
}
