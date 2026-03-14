# IronLedger

[![NuGet](https://img.shields.io/nuget/v/Tudormobile.IronLedgerLib.UI.svg)](https://www.nuget.org/packages/Tudormobile.IronLedgerLib.UI/)
[![License](https://img.shields.io/github/license/tudormobile/IronLedger)](https://github.com/tudormobile/IronLedger/blob/main/LICENSE.txt)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)

**IronLedger** is a comprehensive .NET library for hardware asset identification and inventory management. It provides a robust framework for collecting, serializing, and managing hardware component data across your infrastructure.

## 🎯 Overview

IronLedgerLib enables applications and services to:
- **Collect hardware data** from system components (CPU, memory, disks, motherboard, BIOS)
- **Generate unique asset identifiers** based on hardware fingerprints
- **Serialize and deserialize** asset data using flexible abstractions
- **Track hardware inventory** with strongly-typed, immutable data structures

IronLedgerLib.UI enables applications and services to:
- **Observable Entities** from system components (CPU, memory, disks, motherboard, BIOS)
- **Relay Commands** for management of entities
- **Service Layer** for orchestration via MVVM patterns.

Perfect for building windows desktop UI applications that leverage the Iron Ledger library.

## ⚙️ Platform Support

| Platform | Status |
|----------|--------|
| **Windows** | ✅ Fully Supported |
| **Linux** | Not supported or planned |
| **macOS** | No supported or planned |


## 📦 Installation

Install via NuGet Package Manager:

```bash
dotnet add package Tudormobile.IronLedgerLib.UI
```

Or via Package Manager Console:

```powershell
Install-Package Tudormobile.IronLedgerLib.UI
```

Or add directly to your `.csproj`:

```xml
<PackageReference Include="Tudormobile.IronLedgerLib.UI" Version="1.0.0" />
```

## 🚀 Quick Start

```csharp
using Tudormobile.IronLedgerLib;
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

## 🔧 Requirements

- **.NET 10.0** or higher
- **Windows OS** designed for windows desktop UI applications
- **Administrator privileges** recommended for accessing some hardware data

## 🤝 Contributing

Contributions are welcome! Please read our [Contributing Guidelines](https://github.com/tudormobile/IronLedger/blob/main/CONTRIBUTING.md) before submitting pull requests.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/tudormobile/IronLedger/blob/main/LICENSE) file for details.

## 🔗 Links

- **GitHub Repository:** https://github.com/tudormobile/IronLedger
- **Issue Tracker:** https://github.com/tudormobile/IronLedger/issues
- **Documentation:** https://github.com/tudormobile/IronLedger/tree/main/docs
- **NuGet Package:** https://www.nuget.org/packages/Tudormobile.IronLedgerLib.UI/

## 💡 Use Cases

IronLedgerLib is ideal for:

- **Asset Management Systems** - Track hardware across your infrastructure
- **License Enforcement** - Hardware-based license validation
- **IT Inventory** - Automated hardware inventory collection
- **Configuration Management** - Infrastructure as code with hardware tracking
- **Audit & Compliance** - Hardware change detection and reporting
- **Device Registration** - Unique device identification for cloud services

---

**Built with ❤️ by Tudormobile**