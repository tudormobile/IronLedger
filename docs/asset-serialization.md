# IronLedger Serialization

## Overview

IronLedger provides a flexible serialization abstraction through the `IIronLedgerSerializer` interface. This allows consumers to use their own serialization format while the library provides a default JSON implementation.

## Quick Start

### Using the Default JSON Serializer

```csharp
using Tudormobile.IronLedgerLib;

// Create AssetId
var factory = new AssetIdFactory();
var assetId = factory.Create();

// Serialize using extension method (uses default JSON serializer)
string json = assetId.Serialize();
Console.WriteLine(json);

// Deserialize
AssetId? restored = json.DeserializeAssetId();
```

### Serializing Component Data

```csharp
using Tudormobile.IronLedgerLib;

// Create component data
var componentFactory = new ComponentDataFactory();
var allComponents = componentFactory.Create();

// Serialize entire system
string json = allComponents.Serialize();

// Or serialize individual components
string processorsJson = componentFactory.GetProcessors().First().Serialize();
```

## Custom Serialization

### Implementing a Custom Serializer

```csharp
using Tudormobile.IronLedgerLib;

public class XmlIronLedgerSerializer : IIronLedgerSerializer
{
    public string Serialize<T>(T value)
    {
        // Your XML serialization logic
        var serializer = new XmlSerializer(typeof(T));
        using var writer = new StringWriter();
        serializer.Serialize(writer, value);
        return writer.ToString();
    }

    public T? Deserialize<T>(string data)
    {
        // Your XML deserialization logic
        var serializer = new XmlSerializer(typeof(T));
        using var reader = new StringReader(data);
        return (T?)serializer.Deserialize(reader);
    }
}
```

### Using a Custom Serializer

```csharp
// Create your custom serializer
var xmlSerializer = new XmlIronLedgerSerializer();

// Use with extension methods
var assetId = factory.Create();
string xml = assetId.Serialize(xmlSerializer);

// Deserialize
AssetId? restored = xml.DeserializeAssetId(xmlSerializer);
```

## Direct JSON Serializer Usage

```csharp
using Tudormobile.IronLedgerLib.Serialization;

// Create serializer with default options
var serializer = new IronLedgerJsonSerializer();

// Serialize
var assetId = factory.Create();
string json = serializer.Serialize(assetId);

// Deserialize
AssetId? restored = serializer.Deserialize<AssetId>(json);
```

## Custom JSON Options

```csharp
using System.Text.Json;
using Tudormobile.IronLedgerLib.Serialization;

// Create custom options
var options = new JsonSerializerOptions
{
    WriteIndented = false,  // Compact JSON
    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
};

// Create serializer with custom options
var serializer = new IronLedgerJsonSerializer(options);

// Use it
string compactJson = serializer.Serialize(assetId);
```

## Default JSON Configuration

The default JSON serializer uses the following configuration:

- **WriteIndented**: `true` (formatted for readability)
- **PropertyNamingPolicy**: `CamelCase` (JavaScript-friendly)
- **DefaultIgnoreCondition**: `WhenWritingNull` (omit null values)
- **Converters**: Includes `JsonStringEnumConverter` for enum serialization and `ComponentPropertyConverter` for simplified json property lists.

## API Reference

### IIronLedgerSerializer Interface

```csharp
public interface IIronLedgerSerializer
{
    string Serialize<T>(T value);
    T? Deserialize<T>(string data);
}
```

### Extension Methods

```csharp
// Serialization
string json = assetId.Serialize(serializer);
string json = componentData.Serialize(serializer);
string json = systemComponentData.Serialize(serializer);

// Deserialization
AssetId? assetId = json.DeserializeAssetId(serializer);
ComponentData? component = json.DeserializeComponentData(serializer);
SystemComponentData? system = json.DeserializeSystemComponentData(serializer);
```

All extension methods have an optional `serializer` parameter. If not provided, the default JSON serializer is used.

## Exception Handling

```csharp
try
{
    string json = assetId.Serialize();
}
catch (ArgumentNullException ex)
{
    // Value to serialize was null
}

try
{
    var assetId = json.DeserializeAssetId();
}
catch (ArgumentException ex)
{
    // JSON string was null, empty, or whitespace
}
catch (JsonException ex)
{
    // Invalid JSON format
}
```

## Examples

### Complete Round-Trip Example

```csharp
using Tudormobile.IronLedgerLib;

// Create factories
var assetFactory = new AssetIdFactory();
var componentFactory = new ComponentDataFactory();

// Get data
var assetId = assetFactory.Create();
var components = componentFactory.Create();

// Serialize
string assetJson = assetId.Serialize();
string componentsJson = components.Serialize();

// Save to files
File.WriteAllText("asset.json", assetJson);
File.WriteAllText("components.json", componentsJson);

// Load from files
string loadedAssetJson = File.ReadAllText("asset.json");
string loadedComponentsJson = File.ReadAllText("components.json");

// Deserialize
var restoredAsset = loadedAssetJson.DeserializeAssetId();
var restoredComponents = loadedComponentsJson.DeserializeSystemComponentData();

// Verify
Console.WriteLine($"Asset Serial: {restoredAsset?.SystemMetadata.SerialNumber}");
Console.WriteLine($"Processor Count: {restoredComponents?.Processors.Count}");
```

### Using Different Serializers

```csharp
// Use JSON for web APIs
var jsonSerializer = new IronLedgerJsonSerializer();
string json = assetId.Serialize(jsonSerializer);

// Use custom serializer for file storage
var xmlSerializer = new XmlIronLedgerSerializer();
string xml = assetId.Serialize(xmlSerializer);

// Use custom binary serializer for performance
var binarySerializer = new BinaryIronLedgerSerializer();
string base64 = assetId.Serialize(binarySerializer);
```

## Best Practices

1. **Use extension methods** for convenience when using the default JSON serializer
2. **Pass custom serializer instances** when you need control over format or options
3. **Cache serializer instances** - they're designed to be reused
4. **Handle exceptions** appropriately in production code
5. **Validate deserialized data** before using it in critical operations
