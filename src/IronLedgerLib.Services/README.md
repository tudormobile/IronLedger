# IronLedger

[![NuGet](https://img.shields.io/nuget/v/Tudormobile.IronLedgerLib.Services.svg)](https://www.nuget.org/packages/Tudormobile.IronLedgerLib.Services/)
[![License](https://img.shields.io/github/license/tudormobile/IronLedger)](https://github.com/tudormobile/IronLedger/blob/main/LICENSE.txt)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)

**IronLedger** is a comprehensive .NET library for hardware asset identification and inventory management. It provides a robust framework for collecting, serializing, and managing hardware component data across your infrastructure.

## 🎯 Overview

IronLedgerLib enables applications and services to:
- **Collect hardware data** from system components (CPU, memory, disks, motherboard, BIOS)
- **Generate unique asset identifiers** based on hardware fingerprints
- **Serialize and deserialize** asset data using flexible abstractions
- **Track hardware inventory** with strongly-typed, immutable data structures

IronLedgerLib.Services enables applications and services to:
- **Host web services** to manage Iron Ledger assets and components
- **Build Web clients** to utilize these services.

Perfect for building web applications and web service layer for asset management leveraging the Iron Ledger library.

## ⚙️ Platform Support

| Platform | Status |
|----------|--------|
| **Windows** | ✅ Supported |
| **Linux** | ✅ Supported (with custom providers) |
| **macOS** | ✅ Supported (with custom providers) |

## 📦 Installation

Install via NuGet Package Manager:

```bash
dotnet add package Tudormobile.IronLedgerLib.Services
```

Or via Package Manager Console:

```powershell
Install-Package Tudormobile.IronLedgerLib.Services
```

Or add directly to your `.csproj`:

```xml
<PackageReference Include="Tudormobile.IronLedgerLib.Services" Version="1.0.0" />
```

## 🚀 Quick Start

### Generate a Unique Asset ID

```csharp
using Tudormobile.IronLedgerLib.Services;
```
### Client side access

```csharp
using Tudormobile.IronLedgerLib.Services;
```

## ✨ Key Features

### 🧩 Extensible Architecture
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

## 🤝 Contributing

Contributions are welcome! Please read our [Contributing Guidelines](https://github.com/tudormobile/IronLedger/blob/main/CONTRIBUTING.md) before submitting pull requests.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/tudormobile/IronLedger/blob/main/LICENSE) file for details.

## 🔗 Links

- **GitHub Repository:** https://github.com/tudormobile/IronLedger
- **Issue Tracker:** https://github.com/tudormobile/IronLedger/issues
- **Documentation:** https://github.com/tudormobile/IronLedger/tree/main/docs
- **NuGet Package:** https://www.nuget.org/packages/Tudormobile.IronLedgerLib.Services/

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