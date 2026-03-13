# IronLedger

[![Build and Deploy (main)](https://github.com/tudormobile/IronLedger/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/tudormobile/IronLedger/actions/workflows/dotnet.yml)
[![Publish Docs (main)](https://github.com/tudormobile/IronLedger/actions/workflows/docs.yml/badge.svg?branch=main)](https://github.com/tudormobile/IronLedger/actions/workflows/docs.yml)
[![NuGet](https://img.shields.io/nuget/v/Tudormobile.IronLedgerLib.svg)](https://www.nuget.org/packages/Tudormobile.IronLedgerLib/)
[![License](https://img.shields.io/github/license/tudormobile/IronLedger)](LICENSE.txt)

IronLedger is a .NET library for hardware asset identification and inventory collection.

It helps you:

- Generate deterministic hardware-based asset identifiers.
- Collect system/component metadata for inventory workflows.
- Serialize and share collected asset data.

## Quick Start

Install the package:

```bash
dotnet add package Tudormobile.IronLedgerLib
```

Create an asset identifier:

```csharp
using Tudormobile.IronLedgerLib;

var factory = new AssetIdFactory();
var assetId = factory.Create();

Console.WriteLine(assetId.Id);
```

## Repository Layout

- [src/](src): source projects and tests (library, unit tests, and solution).
- [docs/](docs): documentation source and generated site artifacts.
- [tools/](tools): repository tooling and utilities.

As the project grows, additional libraries and test projects are added under [src/](src) and hosted by the existing solution.

## Learn More

- [Library README](src/IronLedgerLib/README.md)
- [Project documentation](docs/README.md)
- [Source layout guide](src/README.md)

## Contributing

Issues and pull requests are welcome.