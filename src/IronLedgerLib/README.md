# IronLedger

[![NuGet](https://img.shields.io/nuget/v/Tudormobile.IronLedgerLib.svg)](https://www.nuget.org/packages/Tudormobile.IronLedgerLib/)
[![License](https://img.shields.io/github/license/tudormobile/IronLedger)](https://github.com/tudormobile/IronLedger/blob/main/LICENSE.txt)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)

**IronLedger** is a comprehensive .NET library for hardware asset identification and inventory management. It provides a robust framework for collecting, serializing, and managing hardware component data across your infrastructure.

## 🎯 Overview

IronLedgerLib enables applications and services to:
- **Collect hardware data** from system components (CPU, memory, disks, motherboard, BIOS)
- **Generate unique asset identifiers** based on hardware fingerprints
- **Serialize and deserialize** asset data using flexible abstractions
- **Track hardware inventory** with strongly-typed, immutable data structures

Perfect for building asset management systems, license enforcement, hardware tracking, IT inventory solutions, and infrastructure automation tools.

## ⚙️ Platform Support

| Platform | Status |
|----------|--------|
| **Windows** | ✅ Fully Supported |
| **Linux** | 🚧 Planned for future releases |
| **macOS** | 🚧 Planned for future releases |

> **Note:** While the library runs on all .NET-supported platforms, hardware data collection currently uses Windows Management Instrumentation (WMI/CIM) and is fully functional only on Windows. Cross-platform providers for Linux and macOS are planned for future releases.

## 📦 Installation

Install via NuGet Package Manager:

```bash
dotnet add package Tudormobile.IronLedgerLib
```

Or via Package Manager Console:

```powershell
Install-Package Tudormobile.IronLedgerLib
```

Or add directly to your `.csproj`:

```xml
<PackageReference Include="Tudormobile.IronLedgerLib" Version="1.0.0" />
```

## 🚀 Quick Start

### Generate a Unique Asset ID

```csharp
using Tudormobile.IronLedgerLib;

// Create an asset ID from system hardware
var factory = new AssetIdFactory();
var assetId = factory.Create();

Console.WriteLine($"Asset ID: {assetId.Id}");
Console.WriteLine($"System: {assetId.SystemMetadata.Manufacturer} {assetId.SystemMetadata.Product}");
Console.WriteLine($"Serial: {assetId.SystemMetadata.SerialNumber}");
```

### Collect Hardware Inventory

```csharp
using Tudormobile.IronLedgerLib;

// Collect all system component data
var factory = new ComponentDataFactory();
var inventory = factory.Create();

// Access processor information
foreach (var cpu in inventory.Processors)
{
    Console.WriteLine($"CPU: {cpu.Caption}");
    Console.WriteLine($"Manufacturer: {cpu.Metadata.Manufacturer}");

    foreach (var prop in cpu.Properties)
    {
        Console.WriteLine($"  {prop.Name}: {prop.Value}");
    }
}

// Access memory modules
foreach (var memory in inventory.Memory)
{
    Console.WriteLine($"Memory: {memory.Caption}");
}

// Access disk drives
foreach (var disk in inventory.Disks)
{
    Console.WriteLine($"Disk: {disk.Caption}");
}
```

### Serialize Asset Data

```csharp
using Tudormobile.IronLedgerLib;
using Tudormobile.IronLedgerLib.Serialization;

var factory = new AssetIdFactory();
var assetId = factory.Create();

// Serialize to JSON
var serializer = new IronLedgerJsonSerializer();
string json = serializer.Serialize(assetId);

// Deserialize from JSON
var restored = serializer.Deserialize<AssetId>(json);

// Extension methods for convenience
string jsonData = assetId.Serialize();
var restoredAsset = jsonData.DeserializeAssetId();
```

## ✨ Key Features

### 🔑 Unique Asset Identification
- Generate cryptographic hardware fingerprints using SHA256
- Combine system, baseboard, and BIOS metadata
- Consistent, deterministic asset IDs across reboots
- Hexadecimal string representation for easy storage and comparison

### 📊 Hardware Data Collection
- **Processors** - CPU details including cores, speed, and specifications
- **Memory Modules** - RAM information with capacity and type
- **Disk Drives** - Storage device data including size and model
- **System Information** - Manufacturer, model, and serial numbers
- **Baseboard** - Motherboard details and identifiers
- **BIOS** - Firmware version and vendor information

### 🛡️ Robust Exception Handling
- Custom exception hierarchy for precise error handling
- `ComponentDataProviderException` for hardware query failures
- Comprehensive error context with provider name and WMI class information
- Standard .NET exceptions for argument validation

### 📝 Flexible Serialization
- Abstraction-based serialization framework
- Built-in JSON serializer with System.Text.Json
- Snake case naming convention for JSON properties
- Custom converters for component properties
- Easy to implement custom serializers

### 🔒 Immutable Data Structures
- Thread-safe, immutable record types
- Init-only properties prevent accidental modification
- Structural equality for easy comparison
- Support for non-destructive mutation using `with` expressions

### 🧩 Extensible Provider Architecture
- Interface-based provider system
- Easy to implement custom data providers
- Dependency injection-friendly
- Mock providers for testing

## 📖 Documentation

- **[Complete Documentation](https://github.com/tudormobile/IronLedger/tree/main/docs)** - Comprehensive guides and API reference
- **[Exception Handling Guide](https://github.com/tudormobile/IronLedger/blob/main/docs/exception-handling.md)** - Exception handling strategy and best practices
- **[Serialization Guide](https://github.com/tudormobile/IronLedger/blob/main/docs/serialization.md)** - Serialization patterns and customization
- **[API Documentation](https://github.com/tudormobile/IronLedger/tree/main/docs/api)** - Generated API reference

## 🎓 Usage Examples

### Custom Data Providers

Implement custom providers for testing or alternative data sources:

```csharp
public class MockComponentDataProvider : IComponentDataProvider
{
    public IReadOnlyList<ComponentData> GetData()
    {
        return new[]
        {
            new ComponentData
            {
                Metadata = new AssetMetadata
                {
                    SerialNumber = "TEST-001",
                    Manufacturer = "Test Corp",
                    Product = "Test Product"
                },
                Caption = "Test Component",
                Properties = new[]
                {
                    new ComponentProperty("Property1", "Value1"),
                    new ComponentProperty("Property2", "Value2")
                }
            }
        };
    }
}

// Use custom provider
var factory = new ComponentDataFactory(
    processorProvider: new MockComponentDataProvider(),
    systemProvider: null,
    memoryProvider: null,
    diskProvider: null
);
```

### Handle Provider Exceptions

```csharp
try
{
    var factory = new ComponentDataFactory();
    var data = factory.Create();
}
catch (ComponentDataProviderException ex)
{
    Console.WriteLine($"Provider {ex.ProviderName} failed");
    Console.WriteLine($"WMI Class: {ex.WmiClassName}");
    Console.WriteLine($"Error: {ex.Message}");

    // Log and handle gracefully
}
```

### Working with Asset IDs

```csharp
var factory = new AssetIdFactory();
var assetId = factory.Create();

// Access metadata
var systemInfo = assetId.SystemMetadata;
var baseboardInfo = assetId.BaseBoardMetadata;
var biosInfo = assetId.BiosMetadata;

// Get unique identifier
string uniqueId = assetId.UniqueId;
Console.WriteLine($"Hardware Fingerprint: {uniqueId}");

// Modify using with expressions (creates new instance)
var modified = assetId with 
{ 
    SystemMetadata = systemInfo with { SerialNumber = "NEW-SERIAL" }
};

// Compare asset IDs
bool areEqual = assetId == modified; // false - different data
```

## 🔧 Requirements

- **.NET 10.0** or higher
- **Windows OS** for full functionality (WMI/CIM data collection)
- **Administrator privileges** recommended for accessing some hardware data

## 🤝 Contributing

Contributions are welcome! Please read our [Contributing Guidelines](https://github.com/tudormobile/IronLedger/blob/main/CONTRIBUTING.md) before submitting pull requests.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/tudormobile/IronLedger/blob/main/LICENSE) file for details.

## 🔗 Links

- **GitHub Repository:** https://github.com/tudormobile/IronLedger
- **Issue Tracker:** https://github.com/tudormobile/IronLedger/issues
- **Documentation:** https://github.com/tudormobile/IronLedger/tree/main/docs
- **NuGet Package:** https://www.nuget.org/packages/Tudormobile.IronLedgerLib/

## 💡 Use Cases

IronLedgerLib is ideal for:

- **Asset Management Systems** - Track hardware across your infrastructure
- **License Enforcement** - Hardware-based license validation
- **IT Inventory** - Automated hardware inventory collection
- **Configuration Management** - Infrastructure as code with hardware tracking
- **Audit & Compliance** - Hardware change detection and reporting
- **Device Registration** - Unique device identification for cloud services

## 🚧 Roadmap

- ✅ Windows hardware data collection (WMI/CIM)
- ✅ JSON serialization support
- ✅ Comprehensive exception handling
- ✅ Immutable data structures
- 🚧 Linux hardware data providers (planned)
- 🚧 macOS hardware data providers (planned)
- 🚧 Additional serialization formats (XML, MessagePack)
- 🚧 Cloud integration helpers

---

**Built with ❤️ by Tudormobile**