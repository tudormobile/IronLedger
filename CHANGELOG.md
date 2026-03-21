# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Planned
- Improve documentation
- Refinement of the public interface
- Additional logging and error handling

## [0.3.0]

### Added
- Desktop sample application
- Services host sample application (initial version)
- Services client sample application (initial version)

## [0.2.0]

### Added
- Initial data layer
- Introduce service client
- Refine the CI/CD pipeline
- Further build out the services endpoints

## [0.1.0] - 2026-03-15
### Added
- Initial skeleton an minimal implementation
- Core library
- Skeleton services library
- Skeleton UI library
- Minimal tools and documentation
---

## Links

- [NuGet Package](https://www.nuget.org/packages/Tudormobile.IronLedger)
- [GitHub Repository](https://github.com/tudormobile/IronLedger)
- [Documentation](https://tudormobile.github.io/IronLedger)
- [API Documentation](https://tudormobile.github.io/IronLedger/api/Tudormobile.html)

## How to Update This Changelog

### For Maintainers

When making changes, add entries under `## [Unreleased]` in the appropriate category:

- **Added** for new features
- **Changed** for changes in existing functionality
- **Deprecated** for soon-to-be removed features
- **Removed** for now removed features
- **Fixed** for any bug fixes
- **Security** in case of vulnerabilities

When releasing a new version:
1. Change `[Unreleased]` to the version number and date: `[X.Y.Z] - YYYY-MM-DD`
2. Add a new `[Unreleased]` section at the top
3. Commit with message: `chore: release vX.Y.Z`
