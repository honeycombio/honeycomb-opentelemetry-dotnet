# Honeycomb.OpenTelemetry Console example

This example shows how to use the Honeycomb.OpenTelemetry distro with a .NET console application.

To run the example, you need to create an instance of HoneycombOptions and pass it into the `UseHoneycomb` function of the OpenTelemetry TraceProviderBuilder. There are a variety of ways to create the options, as shown below.

### Manually

```csharp
var options = new HoneycombOptions
{
    ServiceName = "my-app",
    ApiKey = "{apikey}",
    Dataset = "{dataset}"
};
```

### Command line arguments

Create options using command line arguments:

```csharp
var options = HoneycombOptionsHelper.GetHoneycombOptions(args);
```

Run app with command line arguments:

`dotnet run --service-name=my-app --honeycomb-apiky={apikey} --honeycomb-dataset={dataset}`

### Configuration files

If you application uses JSON or XML configuration files, you crate an instance of HoneycombOptions to pass to the `UseHoneycomb` function.

```json
{
  "Honeycomb": {
    "ServiceName": "my-app",
    "ApiKey": "{apikey}",
    "Dataset": "{dataset}"
  }
}
```

```xml
<Honeycomb>
  <ServiceName>my-app</ServiceName>
  <ApiKey>{apikey}</ApiKey>
  <Dataset>{dataset}</Dataset>
</Honeycomb>
```

```csharp
var config = new ConfigurationBuilder().AddJsonFile("{setting-file-name}").Build();
var options = HoneycombOptionsHelper.GetHoneycombOptions(configuration);
var tracerProvider = new TracerProviderBuilder()
    .UseHoneycomb(options)
    .Build()
```

### Example

See [Program.cs](Program.cs) for a complete example.
