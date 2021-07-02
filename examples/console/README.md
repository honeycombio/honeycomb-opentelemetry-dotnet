# Honeycomb.OpenTelemetry Console example

This example shows how to use the Honeycomb.OpenTelemetry distro with a .NET console application.

To run the example, you need to create an instance of HoneycombOptions and pass it into the `UseHoneycomb` function of the OpenTelemetry TraceProviderBuilder. There are variety of ways to create the options, as shown below.

### Create instance of HoneycombOptions

```csharp
var options = new HoneycombOptions
{
    ServiceName = "my-app",
    ApiKey = "{apikey}",
    Dataset = "{dataset}"
};
```

Run app:

`dotnet run`

### Command line arguments:

Create options using command line arguments:

```csharp
var options = HoneycombOptionsHelper.GetHoneycombOptions(args);
```

Run app with command line arguments:

`dotnet run --service-name=my-app --honeycomb-apiky={apikey} --honeycomb-dataset={dataset}`

### appsettings.json: 

```csharp
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var options = HoneycombOptionsHelper.GetHoneycombOptions(configuration);
```

```json
{
  "Honeycomb": {
    "ServiceName": "my-app",
    "ApiKey": "{apikey}",
    "Dataset": "{dataset}"
  }
}
```

Run app:
`dotnet run`
