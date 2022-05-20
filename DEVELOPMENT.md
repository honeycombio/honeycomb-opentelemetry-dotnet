## Building & Testing

### Using local packages with other projects

To share this project with another local project you will need to build and store the distributable nuget package in a local nuget source directory. The following makefile target creates a local nuget source, builds and publishes the packages.

`make publish_local`

After the command has run, you can add the package to another project using the normal add package command:

`dotnet add package Honeycomb.OpenTelemetry`
