# Sample Projects

This folder contains sample applications that utilize IronLedger packages.

## Current Layout

- `AssetManager/`: simple asset management desktop application (requires service app).
- `AssetViewer/`: simple asset management desktop application (stand-alone).
- `ServiceClient/`: simple web services client application.
- `ServiceHost/`: simple web services host application.

> [!IMPORTANT]
> Sample applications should reference the IronLedger libraries using ***nuget packages***, not *project references*. 

## Repository Structure Guidance

`samples/` is for sample applications only. All projects in this folder should reference IronLedger libraries using published nuget packages.

Documentation, developer tools, and IronLedger source belong one level up in the repository root:

- `docs/` for docs content and generated site assets.
- `src/` for IronLedger source code.
- `tools/` for command-line utilities and helper tooling.
