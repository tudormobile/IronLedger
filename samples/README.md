# Sample Projects

This folder contains sample applications that utilize IronLedger packages.

## Current Layout

- `AssetViewer/`: simple asset management desktop application (stand-alone).
- `AssetManager/`: simple asset management desktop application (requires service app).
- `ServiceHost/`: simple web services host application.
- `ServiceClient/`: simple web services client application.

> [!IMPORTANT]
> Sample applications should reference the IronLedger libraries using ***nuget packages***, not *project references*. 
>  
> Unlike tools, sample applications must be developed and maintained in a manner consistent with library consumers.

## Repository Structure Guidance

`samples/` is for sample applications only. All projects in this folder should reference IronLedger libraries using published nuget packages.

Documentation, developer tools, and IronLedger source belong one level up in the repository root:

- `docs/` for docs content and generated site assets.
- `src/` for IronLedger source code.
- `tools/` for command-line utilities and helper tooling.
