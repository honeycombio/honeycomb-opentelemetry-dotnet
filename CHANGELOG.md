# honeycomb-opentelemetry-dotnet changelog

## [0.13.0-beta] - 2021-08-26

## Added

- Support for netcoreapp2.1 (#99) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)
- Rename `UseHoneycomb` to `AddHoneycomb` IServiceCollection extension method to be more idiomatic (#98) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)

## Maintenance

- Allow dependabot and forked PRs to run in CI (#97) | [@vreynolds](https://github.com/vreynolds)
- Add issue and PR templates (#91) | [@vreynolds](https://github.com/vreynolds)
- Add OSS lifecycle badge (#90) | [@vreynolds](https://github.com/vreynolds)
- Add community health files (#89) | [@vreynolds](https://github.com/vreynolds)
- Trim commit hash from honeycomb.distro.version attribute (#87) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)

## Dependencies

- Bump Microsoft.NET.Test.Sdk from 16.7.1 to 16.11.0 (#92)

## [0.12.0-beta] - 2021-07-29

## Added

- Add ASP.NET instrumentation (#76, #80) | [@vreynolds](https://github.com/vreynolds) [@MikeGoldsmith](https://github.com/MikeGoldsmith)

## Maintenance

- Only sign builds when publishing through CI (#81) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)
- Clarify releasing steps (#79) | [@vreynolds](https://github.com/vreynolds)

## [0.11.0-alpha] - 2021-07-27

### Added

- Redis instrumentation  (#66, #74) | [@vreynolds](https://github.com/vreynolds)

## [0.10.0-alpha] - 2021-07-15

### Added

- Initial preview release of Honeycomb's OpenTelemetry distribution for .NET!

### Breaking Changes

- As of this version, this project is now Honeycomb's OpenTelemetry distribution for .NET.
  If you are still using the [Honeycomb exporter](https://github.com/honeycombio/opentelemetry-dotnet) use version 0.9.0-pre.

## [0.9.0-pre]

- Pre-release of an OpenTelemetry Exporter for Honeycomb.
  This was the last release of the Exporter published under the Honeycomb.OpenTelemetry Package name.
