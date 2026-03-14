---
_layout: landing
---

# Tudormobile.IronLedger


## 📚 Documentation

- **[Introduction](introduction.md)** - Architecture overview and roadmap  
- **[Project Overview](project-overview.md)** - Overview of the project features and goals  
- **[Exception Handling](exception-handling.md)** - Exception handling strategy and best practices  
- **[Serialization](serialization.md)** - Serialization abstraction and default Json serialization  
- **[Tools](tools.md)** - Reference documentation for utilities in the `tools/` folder  
- **[Generated API Docs](api/Tudormobile.md)** - Auto-generated technical reference

## 🚀 Quick Start

```cs
using Tudormobile.IronLedgerLib;

// Create an asset ID from system hardware
var factory = new AssetIdFactory();
var assetId = factory.Create();

Console.WriteLine($"Asset ID: {assetId.Id}");
```

## 🔧 Build Documentation

[Building the documentation](README.md) locally using DocFX.    

---

**Links:** [`Source Code`](https://github.com/tudormobile/IronLedger) 

