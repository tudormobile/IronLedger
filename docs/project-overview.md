# IronLedger
IT Asset Management, tracking, and disposal.

## Project Overview
IronLedger is an integrated hardware management and security platform. It provides a centralized "Single Source of Truth" for an organization's hardware assets, tracking them from initial procurement through internal component changes to secure end-of-life disposal.

### Core Functional Goals

- **Automated Inventory**: High-fidelity hardware discovery using native system interfaces.
- **Component Tracking**: Granular tracking of internal parts (RAM, Disks) via unique serial numbers.
- **Secure Disposition**: Integrated workflows for data sanitization (NIST 800-88) and auditable decommissioning.
- **Auditable Ledger**: A searchable history of every asset's lifecycle and chain of custody.

### High-Level Architecture
The system utilizes a hybrid model to balance local hardware access with centralized control:

- **Collector**: A series of local software utilities that targets the CIM (Common Information Model) API to extract deep hardware data (Serial Numbers, Disk IDs) with high performance and strong typing.
- **API Gateway**: A secure REST interface that receives encrypted hardware payloads from collectors.
- **The Registry (Database)**: A relational data backend that maintains asset histories, component links, and sanitization certificates.
- **Sanitize**: Local software utilities to securely erase and sanitize to NIST 800-88 standards.
- **Management Dashboard**: A web-based interface for ITAD (Asset Disposition) reporting and auditing.

### Database Schema (The Registry)
The database is structured to support NIST compliance and Component Mobility/Migrations:
- **Assets Table**: Tracks the main chassis/system identity.
- **Components Table**: Tracks individual parts (Drives, RAM) linked to an Asset ID.
- **Sanitization Log**: Stores the definitive proof of wipe (Method, Result, Cert Hash).
- **Assignment Table**: Tracks the history of locations for each asset and components.

### Primary API Interfaces
#### Administrative Endpoints
- `GET /api/v1/status`: Service status.
- `GET /api/v1/assets/{asset_id}`: Retrieves asset or asset list.
#### Ingest Endpoints
- `POST /api/v1/assets/ingest`: Full hardware snapshot submission.
#### Component Update Endpoints
- `GET /api/v1/assets/{asset_id}/components`: Retrieves components associated with an asset.
- `PATCH /api/v1/assets/{asset_id}/components`: Specific component update (for creation and maintenance).
#### Notes Update Endpoints
- `GET /api/v1/assets/{asset_id}/notes`: Retrieves notes associated with an asset.
- `PATCH /api/v1/assets/{asset_id}/notes`: Update notes associated with an asset.
#### Sanitization and Disposal Endpoints [PLANNED]
- `POST /v1/disposal/logs`: Submission of sanitization results and certificates.
- `GET /v1/disposal/pending`: List of assets and components pending disposal.
- `PUT /v1/assets/{asset_id}/status`: Updates asset state (active/retired/disposed)

### Asset Identification
Asset identification based on local inspection can be problematic as assets vary in the amount of unique identifying information that can be reliably extracted by software running on the machine. Since the IronLedger system relies on this type of identification, a system of asset identification is used.

#### Asset Metadata
Assets are identified by a collection of available metadata.
- **System Metadata**. The serial number, product, and manufacturer of the system.
- **Baseboard Metadata**. The baseboard (motherboard) serial number, product, and manufacturer.
- **BIOS Metadata**. The serial number, product, and manufacturer of the BIOS/firmware.

The identifier type used for asset identification is computed from all of the asset metadata listed above. It is still possible for a collision to take place, especially when serial numbers are not available. This must be accounted for in the system architecture typically by presenting the user with the final determination of which specific asset is being identified.
```json
{
  "id": "68e129f573a1758c8ec462e75d0234a30c46c9f8a99359d4c75cfb2a7f0ce4f7",
  "system_metadata": {
    "serial_number": "",
    "manufacturer": "Microsoft Corporation",
    "product": "Microsoft Surface Laptop, 7th Edition"
  },
  "base_board_metadata": {
    "serial_number": "A12345678909876543",
    "manufacturer": "Microsoft Corporation",
    "product": "Microsoft Surface Laptop, 7th Edition"
  },
  "bios_metadata": {
    "serial_number": "F6655443322211",
    "manufacturer": "Microsoft Corporation",
    "product": "175.678.123 QCOM   - 8888"
  }
}
```
### Component Identification
Components are identified by their respective metadata (serial number, manufacturer, and product) in a similar manner to assets, favoring serial numbers where available as a unique identifier while falling back to a hash of the metadata. Component identifiers are not intended to be used as globally unique keys; they only need to be unique for each asset they are associated with, and only for identification purposes even in that case. The exception being components that require secure disposition/disposal, such as disk drives, which require a serial number to participate in that process.
```json
{
  "system": { ... },
  "disks": [
    {
      "metadata": {
        "serial_number": "E823_8FA6_BF53_0001_001B_448B_405D_F2E0.",
        "manufacturer": "(Standard disk drives)",
        "product": "SDDPTQD-1T00-1124-WD"
      },
      "caption": "SDDPTQD-1T00-1124-WD",
      "properties": { ... }
    },
    { ... }
  ],
  "processors": [ ... ],
  "memory": [ ... ],
  ...
}
```

> [!NOTE]
> Refer to the *tools* folder for applications that demonstrate 
> asset and component identification.
