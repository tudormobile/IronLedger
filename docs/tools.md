# IronLedger Tools

Reference documentation for utilities in the `tools/` folder.

## Purpose
This document provides lightweight, tool-by-tool notes for development and validation utilities used by IronLedger.

It is intentionally compact and structured to scale as additional tools are added.

## Conventions
- Keep each tool entry minimal and implementation-focused.
- Prefer practical usage examples over long design descriptions.
- Add new tools to the index and create a matching section using the standard template.

## Tool Index
| Tool | Folder | Status | Primary Purpose |
| --- | --- | --- | --- |
| AssestId | `tools/AssestId` | Active | Generates a stable asset identification payload from local hardware metadata. |

---

## Tool: AssestId

### Summary
Console utility that queries local Windows CIM/WMI data and emits an `AssetId` payload in JSON.

### Current Scope
- Reads baseboard, BIOS, and computer system metadata.
- Builds asset metadata fields used for identity selection (`sn`, `did`, fallback generated id).
- Prints both a selected identifier string and full JSON payload.

### Tech/Dependencies
- Runtime: .NET `net10.0`
- Package: `Microsoft.Management.Infrastructure`
- Platform expectation: Windows environment with CIM/WMI availability.

### Run
From repository root:

```powershell
dotnet run --project .\tools\AssestId\AssestId.csproj
```

### Output
- Human-readable identifier line (`AssetId: ...`)
- JSON payload containing the serialized `AssetId` model, including:
	- System metadata (e.g., computer system identity details used for asset identification)
	- Baseboard metadata (e.g., board serial and related identifiers)
	- BIOS metadata (e.g., firmware identity/build information)
- The exact JSON schema is defined by the `AssetId` type in `tools/AssestId`.

### Notes
- Current folder/project naming uses `AssestId` (spelling retained to match codebase).
- Fallback text values are used when metadata cannot be read from CIM queries.

---

## New Tool Template
Use this section pattern for future tools.

```markdown
## Tool: <ToolName>

### Summary
<1-2 sentence purpose>

### Current Scope
- <capability>
- <capability>

### Tech/Dependencies
- Runtime: <framework/runtime>
- Package(s): <if any>
- Platform expectation: <os/environment>

### Run
~~~powershell
dotnet run --project .\tools\<ToolFolder>\<ToolProject>.csproj
~~~

### Output
- <key output 1>
- <key output 2>

### Notes
- <limitations, caveats, or naming notes>
```
