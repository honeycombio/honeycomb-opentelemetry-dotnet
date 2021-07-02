# Honeycomb.OpenTelemetry ASP.NET Core web example

This example shows how to use the Honeycomb.OpenTelemetry distro with a .NET Core web application.

To run the example, you need to set your Honeycomb API key and a dataset name in [appsettings.json](appsettings.json) (see main [README](../../README.md) for details), then you can run the example with:

`dotnet run`

As part of the configuration process, an instance of the Tracer is registed in the services Dependency Injection map that can be be injected into controllers and used to create additional spans. See the [WeatherForecastController](Controllers/WeatherForecastController.cs) for an example.
