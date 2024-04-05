# Honeycomb.OpenTelemetry Console example

This example shows how to use the Honeycomb OpenTelemetry components with the standard OpenTelemetry .NET SDK in a console application.

### Manually

```csharp
var options = new HoneycombOptions
{
    ServiceName = "my-app",
    ApiKey = "{apikey}",
    Dataset = "{dataset}"
};
```

### Configuration

Some of the Honeycomb components, such as the OTLP exporter and local visualization span processor, require a Honeycomb API key. You can choose how that's passed into your application, eg environment variables or configuration file.

### How to run

```dotnet
dotnet run
```

### Example

See [Program.cs](Program.cs) for a complete example.
