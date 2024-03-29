# Honeycomb.OpenTelemetry ASP.NET Core with Redis web example

This example shows how to use the Honeycomb OpenTelemetry distro with a .NET Core web application.
This example also includes automatic instrumentation of Redis.

## Using the Example

- Install Redis and start the service, for example `brew services start redis`
- Set your Honeycomb API key and a service name in the [appsettings.json](appsettings.json) configuration file.
- If you are using Honeycomb Classic, add `TracesDataset`.

Run `dotnet run` to start the app.

To generate telemetry, navigate to `localhost:5001/weatherforecast`.

![my-web-app-with-redis](./my-web-app-with-redis.png)

As part of the configuration process, an instance of the Tracer is registed in the services Dependency Injection map that can be be injected into controllers and used to add additional context and create additional spans.

See [Startup.cs](Startup.cs) for an example of how the `AddHoneycomb` method is called.

See [WeatherForecastController](Controllers/WeatherForecastController.cs) for an example app being instrumented.
