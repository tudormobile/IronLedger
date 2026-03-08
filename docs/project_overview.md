# IronLedger
IT Asset Management, tracking, and diposal.

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
- **Sanatize**: Local software utilities to securely erase and sanatize to NIST 800-88 standards.
- **Management Dashboard**: A web-based interface for ITAD (Asset Disposition) reporting and auditing.

### Database Schema (The Registry)
The database is structured to support NIST compliance and Component Mobility/Migrations:
- **Assets Table**: Tracks the main chassis/system identity.
- **Components Table**: Tracks individual parts (Drives, RAM) linked to an Asset ID.
- **Sanitization Log**: Stores the definitive proof of wipe (Method, Result, Cert Hash).
- **Assignment Table**: Tracks the history of locations for each asset and components.

### Primary API Interfaces
#### Ingest and Discovery Endpoints
- `POST /v1/assets/ingest`: Full hardware snapshot submission.
- `GET /v1/assets/{asset_id}`: Retrieves asset or asset list.
- `PATCH /v1/assets/{id}/components`: Specific component update (for maintenance).
#### Sanitaztion and Disposal Endpoints
- `POST /v1/disposal/logs`: Submission of sanitization results and certificates.
- `GET /v1/disposal/pending`: List of assets and components pending disposal.
- `PUT /v1/assets/{asset_id}/status`: Updates asset state (active/retired/disposed)

### Asset Identification
Asset identification based on local inspection can be problematic as assets vary in the amount of unique identifying information that can be reliably extracted by software running on the machine. Since the IronLedger system relies on this type of identification, a system of asset identification is used.

#### Asset Metadata
Assets are identified by a prioritized series of available metadata.
- **Serial Numbers**. When available, serial numbers are used to identify assets and components.
- **Device Identifiers**. Used when serial numbers cannot be obtained.
- **Device Name**. Used when serial numbers or identifiers are unavailable, provided it is unique.
- **Identifier**. A generated identifier using all available information and guarenteed unique.  

The identifier type used for asset identification will be included in the meta-data of an asset or components, and users may not modify these values during ingest or component modification. All other values may be edited for correctness, completeness, etc. 
```json
{
    "id_type": "sn|did|gid",
    "metadata": {
        "serial_number": "SN-12345-ABC",
        "device_id": "BIOS|American Megatrends Inc.|1801|12/25/2022 7:00:00 PM|ALASKA - 1072009",
        "generated_id": "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
        "name" : "ENG-WORKSTATION-04",
        "manufacturer": "DELL Inc.",
        "model": "Precision 5570"
        ...
    },
    "components": [
        { ... },
        { ... }
    ]
}
```
### Component Identification
Conponents are identified in a similar manner to assets, favoring serial numbers where available while falling back to prioritized list of available attributes, depending on the specific component type.

> [!NOTE]
> Refer to the *tools* folder for applications that demonstrate 
> asset and component identification.
