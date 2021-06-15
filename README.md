# honeycomb-opentelemetry-dotnet

### Building, testing and creating the nuget package

You can execute the following commands to build and test the project:

```bash
dotnet build
dotnet test
```

### Using local packages with othe projects

To share this project with another local project you need build the distributable nuget package and store it in a shared local nuget source.

[publish-local.sh](publish-local.sh) builds and publishes the packages and the script looks for the environment variable `NUGET_PACKAGES_LOCAL` and will default to `$HOME/.nuget/local` if not set.

After executing the script, you then you need to run the following commands in the project you wish to use nuget package:

```bash
dotnet nuget add source $HOME/.nuget/local
dotnet add package Honeycomb.OpenTelemetry
```

NOTE: In the future, the nuget package will be published to nuget.org, where dependency management will be easier and we won't need the local nuget source.