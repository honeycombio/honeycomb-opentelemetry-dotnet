# Honeycomb OpenTelemetry Distro for .NET

[![CircleCI](https://circleci.com/gh/honeycombio/honeycomb-opentelemetry-dotnet.svg?style=shield)](https://circleci.com/gh/honeycombio/honeycomb-opentelemetry-dotnet)

This is Honeycomb's distribution of OpenTelemetry for .NET.
It makes getting started with OpenTelemetry and Honeycomb easier!

**NOTE** This project took over a pre-existing Nuget package `Honeycomb.OpenTelemetry` after version `0.9.0-pre`. 
Version `0.9.0-pre` is the only version that will contain the initial implementation (Honeycomb exporter).

## Why would I want to use this?

- Streamlined configuration for sending data to Honeycomb!
- Easy interop with existing instrumentation with OpenTelemetry!
- Deterministic sampling!
- Multi-span attributes!

## Getting Started

### Installing the Honeycomb.OpenTelemetry package

You can add the package to your application by using the following command:

`dotnet add package Honeycomb.OpenTelemetry`

### Configuration

The two ways to configure the Honeycomb OpenTelemetry SDK distro to send data to Honeycomb are via command line arguments and `IConfiguration` instances (eg via appsettings.json).

The available configuration options are:

|Command line argument|Appsetting key|Description|
|-|-|-|
|`--honeycomb-apikey`|Honeycomb.ApiKey|`required` The API key used to send data|
|`--honeycomb-dataset`|Honeycomb.Dataset|`required` The dataset to store telemetry data in|
|`--honeycomb-samplerate`|Honeycomb.SampleRate|`optional` Defaults to 1 (sample everything)|
|`--honeycomb-endpoint`|Honeycomb.Endpoint|`optioal` Override the endpoint data is sent to|
|`--service-name`|Honeycomb.ServiceName|`optional` Defaults to project's assembly name|
|`--service-version`|Honeycomb.ServieVersion|`optional` Defaults to project's assembly version|

*NOTE* While ServiceName is optional, it can be useful to set it to something that best describes the process that is being traced. eg `auth-service`

Using command line arguments:
```bash
dotnet run --servicename=my-app --honeycomb-apikey={apikey} --honeycomb-dataset={dataset}
```

Using appsettings.json:
```json
{
  "Honeycomb": {
    "ServiceName": "my-app",
    "ApiKey": "{apikey}",
    "Dataset": "{dataset}"
  }
}
```

```bash
dotnet run
```

## Building & Testing

### Prerequisites to Build

You will need to download and install the latest .NET SDK:

- [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)

The following makefile commands build the projects:

`make` or `make build`

The following makefile command executes tests:

`make test`

### Using local packages with other projects

To share this project with another local project you will need to build and store the distributable nuget package in a local nuget source directory. The following makefile target creates a local nuget source, builds and publishes the packages.

`make publish_local`

After the command has run, you can add the package to another project using the normal add package command:

`dotnet add package Honeycomb.OpenTelemetry`

The default location is `${HOME}/.nuget/local` and can overridden to another location by setting the `NUGET_PACKAGES_LOCAL` environment variable.

NOTE: In the future, the nuget package will be published to nuget.org, where dependency management will be easier and we won't need the local nuget source.
