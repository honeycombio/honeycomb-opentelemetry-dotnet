# honeycomb-opentelemetry-dotnet

### Building, testing and creating the nuget package

You can execute the following commands to build and test the project:

```bash
dotnet build
dotnet test
```

### Using local packages with other projects

To share this project with another local project you will need to build and store the distributable nuget package in a local nuget source directory. The following makefile target creates a local nuget source then builds & publishes the packages.

`make install`

The default location is `${HOME}/.nuget/local` and can overridden to another location by setting the `NUGET_PACKAGES_LOCAL` environment variable.

NOTE: In the future, the nuget package will be published to nuget.org, where dependency management will be easier and we won't need the local nuget source.