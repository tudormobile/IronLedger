# Source Projects

This folder contains the .NET source code for IronLedger.

## Current Layout

- `IronLedgerLib/`: core library project.
- `IronLedgerLib.Tests/`: unit tests for the core library.
- `IronLedger.slnx`: solution file that currently hosts the library and tests.

## Repository Structure Guidance

`src/` is for source and test projects only.

When the codebase grows, additional projects should be added here (for example, integration tests or server-side libraries) and included in `IronLedger.slnx`.

Documentation and developer tools belong one level up in the repository root:

- `docs/` for docs content and generated site assets.
- `tools/` for command-line utilities and helper tooling.
