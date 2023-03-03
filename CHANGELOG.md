# honeycomb-opentelemetry-dotnet changelog

## [1.3.0/0.27.0-beta] - 2023-03-02

### !!! Breaking Changes !!!

### Maintenance

- maint: Rename AutoInstrumentations package to CommonInstrumentations (#327) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)
  - renamed the package to make it more clear that this is common instrumentation, not technically "automatic" instrumentation.
  - `Honeycomb.OpenTelemetry.AutoInstrumentations` -> `Honeycomb.OpenTelemetry.CommonInstrumentations`
  - `AddAutoInstrumentations` -> `AddCommonInstrumentations`
- maint: Update OpenTelemetry instrumentation packages (#334) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)
  - The new `OpenTelemetry` packages change how tracing and metrics are setup
  - `AddOpenTelemetryTracing` -> `AddOpenTelemetry().WithTracing`
  - `AddOpenTelemetryMetrics` -> `AddOpenTelemetry().WithMetrics`
- maint(deps): bump OpenTelemetry.Instrumentation.StackExchangeRedis from 1.0.0-rc9.7 to 1.0.0-rc9.8 (#345)
- maint(deps): bump Microsoft.NET.Test.Sdk from 17.3.2 to 17.4.1 (#336)
- maint: new signed snkgpg, use long cmd lines for clarity (#341) | [@JamieDanielson](https://github.com/JamieDanielson)

## [1.2.1/0.26.1-beta] - 2022-11-22

### Fixes

- Only add Redis instrumentation if connection found in DI (#323) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)

## [1.2.0/0.26.0-beta] - 2022-11-16

### Enhancements

- Support IConfiguration with Honeycomb Options (#317) | [@pjwilliams2](https://github.com/pjwilliams2)

### Maintenance

- Remove step from releasing (#312) | [@vreynolds](https://github.com/vreynolds)
- Update README.md (#313) | [@isaacabraham](https://github.com/isaacabraham)
- Fix metrics smoke tests (#320) | [@pkanal](https://github.com/pkanal)

### Dependencies

- Bump instrumentation pacakges to 1.0.0-rc9.9 (#318)
- Update aspnetcore instrumentation to 1.0.0-rc9.9 (#319)
- Bump OpenTelemetry.Instrumentation.MySqlData from 1.0.0-beta.3 to 1.0.0-beta.4 (#314)
- Bump OpenTelemetry.Instrumentation.AspNet from 1.0.0-rc9.5 to 1.0.0-rc9.6 (#289)
- Bump Microsoft.NET.Test.Sdk from 17.3.1 to 17.3.2 (#315)
- Bump coverlet.collector from 3.1.2 to 3.2.0 (#316)

## [1.1.0/0.25.0-beta] - 2022-10-27

### Fixes

- De-bump Npgsql.OpenTelemetry to 6.0.7 to avoid collisions with LTS entityframework (#298) | @cartermp

### Maintenance

- Update OTel hosting & instrumentation packages (#306) | @MikeGoldsmith
- Add F# example? ( ･ω･) (#296) | @cartermp

## [1.0.0/0.24.0-beta] - 2022-09-30

### !!! Breaking Changes !!!

### Enhancements

- Add support for `OTEL_SERVICE_NAME` environment variable (#245) | [@pkanal](https://github.com/pkanal)
- Attempt to match Java distro resources (#249) | [@cartermp](https://github.com/cartermp)
- Don't bundle instrumentation packages in distro (#250) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)
- Support debug option (#254) | [@cartermp](https://github.com/cartermp)
- Add AspNetCore instrumentation extension to add baggage to new spans (#255) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)
- Set options with environment variables if they're present (#261) | [@cartermp](https://github.com/cartermp)
- Add Honeycomb.OpenTelemetry.Autoinstrumentations #270 | [@cartermp](https://github.com/cartermp)
- Append /v1/metrics to endpoint path if the protocol is HTTP (#284) | [@pkanal](https://github.com/pkanal)
- Append /v1/traces path to http endpoint (#279) | [@pkanal](https://github.com/pkanal)
- Remove custom command line argument parsing (#282) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)
- Add ConfigurationManager extensions methods (#280) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)
- Move extensions into owning class namespace & rename add instrumentations ext method (#276)
- Add HONEYCOMB prefix to local visualizations env var (#275) | [@cartermp](https://github.com/cartermp)

### Maintenance

- Specify OTel version in README (#246) | [@pkanal](https://github.com/pkanal)
- Add grpc smoke tests (#264) | [@pkanal](https://github.com/pkanal)
- Add http/protobuf smoke tests (#263) | [@pkanal](https://github.com/pkanal)
- Add make targets for smoke tests & set up CI smoke tests (#268) | [@pkanal](https://github.com/pkanal)
- Dockerize example and prep for smoke tests, target .NET6 (#243) | [@JamieDanielson](https://github.com/JamieDanielson)
- Add Redis instrumentation example app (#242) | [@JamieDanielson](https://github.com/JamieDanielson)
- Move aspnetcore samples to modern .NET 6-style apps (#271) | [@cartermp](https://github.com/cartermp)
- Minor code formatting (#260) | [@cartermp](https://github.com/cartermp)
- Idiomatic code formatting (#247) | [@cartermp](https://github.com/cartermp)
- Add Tracer to DI in aspnet examples (#262) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)
- Update aspnetcore example to send metrics (#281) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)
- Update CircleCI config to publish tags correctly (#286) | [@pkanal](https://github.com/pkanal)

### Dependencies

- Bump xunit from 2.4.1 to 2.4.2 (#259)
- Bump System.Text.Json from 6.0.5 to 6.0.6 (#258)
- Bump xunit.runner.visualstudio from 2.4.3 to 2.4.5 (#257)
- Bump Microsoft.NET.Test.Sdk from 17.2.0 to 17.3.1 (#256)

## [0.23.0-beta] - 2022-08-03

### Enhancements

- Added Console Trace link writer (#222) | [@martinjt](https://github.com/martinjt)

### Maintenance

- Update minimum target framework from net461 to net462 (#226) | [@vreynolds](https://github.com/vreynolds)
- Bump OpenTelemetry.Instrumentation.AspNet from 1.0.0-rc9.1 to 1.0.0-rc9.5 (#224) | [dependabot](https://github.com/dependabot)
- Bump Microsoft.NET.Test.Sdk from 17.1.0 to 17.2.0 (#221) | [dependabot](https://github.com/dependabot)

## [0.22.0-beta] - 2022-07-01

### Enhancements

- Add OTLP version to headers (HTTP & gRPC) (#205) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)

### Maintenance

- Update README to point to docs website for setup instructions (#208) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)
- Bump otel dependencies #213 | [@MikeGoldsmith](https://github.com/MikeGoldsmith)
- Bump Microsoft.NET.Test.Sdk from 17.0.0 to 17.1.0 (#195)

## [0.21.0-beta] - 2022-04-14

### Enhancements

- Capture null options in IServiceColletion.AddHoneycomb (#184) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)

### Fixes

- Add support for providing ResouceBuilder as configuration option (#190) | [@MikeGoldsmith](https://github.com/MikeGoldsmith)

### Maintenance

- Bump otel dependdencies (#193)
- Bump OpenTelemetry.Instrumentation.AspNet from 1.0.0-rc8 to 1.0.0-rc9.1 (#186)
- Bump coverlet.collector from 3.1.0 to 3.1.2 (#181)

## [0.20.0-beta] - 2022-03-07

### Fixes

- Remove white space in default service.name (#182)

## [0.19.0-beta] - 2022-02-09

### Enhancements

- Provide more feedback to users to help configure for use in E&S (#175) | [@JamieDanielson](https://github.com/JamieDanielson)

### Fixes

- Validate that HoneycombOptions exists before we do anything (#171) | [@cartermp](https://github.com/cartermp)

### Maintenance

- gh: add re-triage workflow (#166) | [@vreynolds](https://github.com/vreynolds)

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
