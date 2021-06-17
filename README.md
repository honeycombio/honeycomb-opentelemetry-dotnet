# honeycomb-opentelemetry-dotnet

### Prerequisites

You will need to download and install following .NET SDK runtimes:

- [.NET Core 2.1](https://dotnet.microsoft.com/download/dotnet/2.1)
- [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet/3.1)
- [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)

### Building & Testing

The following makefile commands build the projects:

`make` or `make build`

The following makefile command executes tests:

`make test`

### Using local packages with other projects

To share this project with another local project you will need to build and store the distributable nuget package in a local nuget source directory. The following makefile target creates a local nuget source, builds and publishes the packages.

`make install`

After the command has run, you can add the package to another project using the normal add package command:

`dotnet add package Honeycomb.OpenTelemetry`

The default location is `${HOME}/.nuget/local` and can overridden to another location by setting the `NUGET_PACKAGES_LOCAL` environment variable.

NOTE: In the future, the nuget package will be published to nuget.org, where dependency management will be easier and we won't need the local nuget source.