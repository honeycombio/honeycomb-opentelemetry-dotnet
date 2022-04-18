# Honeycomb.OpenTelemetry Console example

This example shows how to use the Honeycomb.OpenTelemetry distro with a .NET console application.

To run the example, you need to create an instance of HoneycombOptions and pass it into the `AddHoneycomb` function of the OpenTelemetry TraceProviderBuilder. There are a variety of ways to create the options, as shown below.

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

`dotnet run --service-name=my-app --honeycomb-apikey={apikey} --honeycomb-dataset={dataset}`

### Configuration files

If you application uses JSON or XML configuration files, you crate an instance of HoneycombOptions to pass to the `AddHoneycomb` function.

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
    .AddHoneycomb(options)
    .Build()
```

### Setting Resource Attributes

You can set additional resource attributes by either using an environment variable or programmatically using HoneycomnOptions when configuring the SDK.

#### Environment Variable

You can use `OTEL_RESOURCE_ATTRIBUTES` to set extra resoruce attributes. The value is key value pairs and multiple entries are separated using a comma (`,`).

For example:

`export OTEL_RESOURCE_ATTRIBUTES="deployment.environment=dev"`

#### Programatically using HoneycombOptions

You can set resource attributes proramatically by creating your own ResourceBuilder and passing adding it to HoneycombOptions.

```csharp
var resourceBuilder = ResourceBuilder.CreateDefault().AddAttributes(
  new Dictionary<string, object>
  {
      {"deployment.environment", "dev"}
  });
var tracerProvider = OpenTelemetry.Sdk.CreateTracerProviderBuilder()
  .AddHoneycomb(options => options.ResourceBuilder = resourceBuilder)
  .Build();
```

### Example

See [Program.cs](Program.cs) for a complete example.
