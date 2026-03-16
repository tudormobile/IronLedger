# IronLedger Data Layer

This document describes how to persist and retrieve asset data using the IronLedger data layer.

## Overview

The data layer is built around the `IAssetRepository` interface, which provides asynchronous CRUD operations for `AssetRecord` instances and associated Markdown notes. The default implementation, `FileSystemAssetRepository`, stores data as JSON and Markdown files on the local file system - no database or external dependency required.

### Key Types

| Type | Project | Purpose |
|---|---|---|
| `AssetRecord` | `IronLedgerLib` | Combines `AssetId` and `SystemComponentData` into a single storable unit |
| `IAssetRepository` | `IronLedgerLib` | Abstraction for asset persistence |
| `FileSystemAssetRepository` | `IronLedgerLib` | Default file-system implementation |

## Quick Start

### Registering the Repository

Call `AddIronLedgerService` during application startup. `FileSystemAssetRepository` is automatically registered as a singleton `IAssetRepository`.

```csharp
builder.Services.AddIronLedgerService(options =>
{
    options.DataPath = @"C:\ProgramData\IronLedger\assets"; // optional; defaults to ./data
});
```

### Saving an Asset

```csharp
public class InventoryService(IAssetIdFactory idFactory, IComponentDataFactory componentFactory, IAssetRepository repository)
{
    public async Task ScanAndSaveAsync(CancellationToken ct = default)
    {
        var record = new AssetRecord
        {
            Id = idFactory.Create(),
            Components = componentFactory.Create()
        };

        await repository.SaveAsync(record, ct);
    }
}
```

### Retrieving Assets

```csharp
// All assets
IReadOnlyList<AssetRecord> all = await repository.GetAllAsync(ct);

// One asset by its stable SHA-256 identifier
AssetRecord? asset = await repository.GetAsync(assetId, ct);
```

### Working with Markdown Notes

Each asset can have a freeform Markdown file stored alongside its JSON data.

```csharp
// Save notes
await repository.SaveNotesAsync(asset.Id.Id, "# Server Rack 3\n\nReplaced NIC on 2025-06-01.", ct);

// Read notes
string notes = await repository.GetNotesAsync(asset.Id.Id, ct);
```

### Deleting an Asset

Deletes both the JSON file and Markdown notes in a single call.

```csharp
await repository.DeleteAsync(asset.Id.Id, ct);
```

## File System Layout

`FileSystemAssetRepository` creates one subdirectory per asset under the configured `DataPath`. The directory name is the asset's deterministic SHA-256 identifier (see `AssetId.Id`).

```
{DataPath}/
  a3f8c1...d72e/          <- AssetId.Id (64-char hex)
    asset.json            <- serialized AssetRecord
    notes.md              <- optional Markdown notes
  9b04e7...1ac2/
    asset.json
    notes.md
```

`asset.json` is written by the configured `IIronLedgerSerializer` (JSON by default), so files are human-readable and can be committed to version control or inspected without tooling.

## Configuration

`DataPath` is set through `IronLedgerOptions`:

```csharp
builder.Services.AddIronLedgerService(options =>
{
    options.DataPath = "/var/lib/ironledger";
});
```

If `DataPath` is not set, it defaults to a `data` subdirectory of the current working directory.

## Implementing a Custom Repository

Implement `IAssetRepository` to use a different backing store (e.g., LiteDB, Azure Blob Storage) and register it **before** calling `AddIronLedgerService`. The built-in registration uses `TryAddSingleton`, so an existing registration is never overwritten.

```csharp
public class MyCustomRepository : IAssetRepository
{
    public Task<IReadOnlyList<AssetRecord>> GetAllAsync(CancellationToken cancellationToken = default) { ... }
    public Task<AssetRecord?> GetAsync(string assetId, CancellationToken cancellationToken = default) { ... }
    public Task SaveAsync(AssetRecord asset, CancellationToken cancellationToken = default) { ... }
    public Task DeleteAsync(string assetId, CancellationToken cancellationToken = default) { ... }
    public Task<string> GetNotesAsync(string assetId, CancellationToken cancellationToken = default) { ... }
    public Task SaveNotesAsync(string assetId, string markdown, CancellationToken cancellationToken = default) { ... }
}
```

```csharp
// Register before AddIronLedgerService -- TryAddSingleton will not overwrite it
builder.Services.AddSingleton<IAssetRepository, MyCustomRepository>();
builder.Services.AddIronLedgerService();
```

> **Note:** `AddIronLedgerService` uses `TryAddSingleton` internally, so any `IAssetRepository` already in the container is preserved.
