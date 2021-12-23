# honeycomb-opentelemetry-dotnet changelog

## [0.18.0-beta] - 2021-12-23

### Fixes

- remove parent-based sampler, sample with trace ID ratio only (#164) | [@vreynolds](https://github.com/vreynolds)

## [0.17.0-beta] - 2021-12-15

### Changed

- Make API key and dataset optional (#161) | [@vreynolds](https://github.com/vreynolds)

### Maintenance

- Update example core app to use metrics (#158) | [@vreynolds](https://github.com/vreynolds)

### Dependencies

- Bump OpenTelemetry from 1.2.0-beta2.1 to 1.2.0-rc1 (#151)
- Bump OpenTelemetry.Exporter.OpenTelemetryProtocol (#152)

## [0.16.0-beta] - 2021-12-06

### Improvements

- Add support for enabling OTLP metrics (#146) | [@MikeGoldsmith](https://github.com/MikeGoldsmith) [@vreynolds](https://github.com/vreynolds)
- Parse missing command line arguments (#154) | [@vreynolds](https://github.com/vreynolds)

### Fixes

- Re-add net461 target (#155) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)
- Trace extension should use trace config (#156) | [@vreynolds](https://github.com/vreynolds)

## [0.15.0-beta] - 2021-12-01

### Enhancements

- Update DeterminisitcSampler to use OTel core Parent/TraceIDRatio samplers (#145) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)

### Fixes

- Fixed Redis auto-detection (#150) | [@JamieDanielson](https://github.com/JamieDanielson)

### Maintenance

- Update dependabot.yml (#147) | [@vreynolds](https://github.com/vreynolds)
- Update dependencies, remove netcoreapp2.1 (#144) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)
- Update docs when releasing (#142) | [@vreynolds](https://github.com/vreynolds)

## Dependencies

- Bump Microsoft.NET.Test.Sdk from 16.11.0 to 17.0.0 (#148)

## [0.14.0-beta] - 2021-11-18

## Added

- Allow disabling auto-instrumentations (#129) | [@ericsampson](https://github.com/ericsampson)

## Maintenance

- Remove whitespace failing build (#137) | [@JamieDanielson](https://github.com/JamieDanielson)
- Empower apply-labels action to apply labels (#131) | [@robbkidd](https://github.com/robbkidd)
- Add prerelease flag to install command (#120) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)
- Add NOTICE (#114) | [@cartermp](https://github.com/cartermp)
- Only add stale to info needed or revision needed (#115) | [@JamieDanielson](https://github.com/JamieDanielson)
- Adds stalebot effective Sept 1 2021 (#113) | [@JamieDanielson](https://github.com/JamieDanielson)
- Fix netcoreapp2.1 example (#102) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)
- Clarify renaming in changelog (#101) | [@cartermp](https://github.com/cartermp)

## Dependencies

- Bump Microsoft.AspNetCore.Razor.Design from 2.1.2 to 2.2.0 (#104)
- Bump coverlet.collector from 1.3.0 to 3.1.0 (#77)

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
