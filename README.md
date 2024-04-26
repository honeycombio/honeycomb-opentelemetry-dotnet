# Honeycomb OpenTelemetry Distro for .NET

[![OSS Lifecycle](https://img.shields.io/osslifecycle/honeycombio/honeycomb-opentelemetry-dotnet)](https://github.com/honeycombio/home/blob/main/honeycomb-oss-lifecycle-and-practices.md)
[![CircleCI](https://circleci.com/gh/honeycombio/honeycomb-opentelemetry-dotnet.svg?style=shield)](https://circleci.com/gh/honeycombio/honeycomb-opentelemetry-dotnet)
[![Dependencies](https://img.shields.io/librariesio/release/nuget/Honeycomb.Opentelemetry.svg)](https://github.com/honeycombio/honeycomb-opentelemetry-dotnet/blob/main/src/Honeycomb.OpenTelemetry/Honeycomb.OpenTelemetry.csproj)
[![Nuget](https://img.shields.io/nuget/v/Honeycomb.OpenTelemetry.svg)](https://www.nuget.org/packages/Honeycomb.OpenTelemetry)

**STATUS**: This project is being Sunset. See [this issue](https://github.com/honeycombio/honeycomb-opentelemetry-dotnet/issues/193) for more details.

This is Honeycomb's distribution of OpenTelemetry for .NET.
It makes getting started with OpenTelemetry and Honeycomb easier!

Latest release built with:

- [OpenTelemetry Version 1.6.0](https://github.com/open-telemetry/opentelemetry-dotnet/releases/tag/core-1.6.0)

***NOTE***

OpenTelemetry .NET SDK versions after 1.6.0 use updated HTTP semantic conventions that result in different telemetry attribute keys and can contribute to unexpected query results in Honeycomb. For this reason, we have decided the Honeycomb OpenTelemetry distro will not take further updates yet.

Please see our [Migration guide for HTTP semantic conventions](https://docs.honeycomb.io/troubleshoot/product-lifecycle/recommended-migrations/#migrate-to-stabilized-opentelemetry-semantic-conventions) for more details.

## Getting Started

Honeycomb's OpenTelemetry .NET SDK gives you the ability to add manual instrumentation to your applications.

- [Documentation](https://docs.honeycomb.io/getting-data-in/dotnet/opentelemetry-distro/)
- [Examples](/examples/)

## Why would I want to use this?

- Streamlined configuration for sending data to Honeycomb!
- Easy interop with existing instrumentation with OpenTelemetry!
- Deterministic sampling!
- Multi-span attributes!

### Overriding OpenTelemetry SDK Builder options

The OpenTelemetry SDK uses a builder pattern to set options and currently does not provide a way to know if a particular option has already been set.
This can lead to the same option being set multiple times with the last one wins behaviour.

For example, the `AddHoneycomb(options)` function configures a Sampler so another call to `SetSampler(sampler)` will override the first sampler.


### HTTP Semantic Conventions

The semantic conventions for attribute names used in HTTP instrumentation libraries is being updated and could cause distruption for existing users who rely on the existing names.
For this reason, we have locked HTTP instrumentation packages to [1.6.0-beta.3](https://github.com/open-telemetry/opentelemetry-dotnet/releases/tag/1.6.0-beta.3) which includes the `OTEL_SEMCONV_STABILITY_OPT_IN` environment variable that allows users to opt-in to the new behaviour when they are ready to.

See this [Migration Guide](https://docs.honeycomb.io/getting-data-in/semconv/migration) for details on how to switch between the old and new attribute keys when using Honeycomb.

## License

[Apache 2.0 License](./LICENSE).