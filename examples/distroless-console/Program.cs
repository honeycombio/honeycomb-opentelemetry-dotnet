using System;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

var serviceName = Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME");
if (string.IsNullOrEmpty(serviceName))
{
    serviceName = "my-app";
}

var apikey = Environment.GetEnvironmentVariable("HONEYCOMB_API_KEY");
if (string.IsNullOrEmpty(apikey))
{
    Console.WriteLine("WARN: Honeycomb API key not set");
}

var tracerProvider = OpenTelemetry.Sdk.CreateTracerProviderBuilder()
    .AddSource(serviceName)
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(serviceName)
    )
    // add baggage span processor to copy baggage items from parent to child spans
    .AddBaggageSpanProcessor()
    // add deterministic sampler with 1/5 (20%) sample rate
    .AddDeterministicSampler(5)
    // add local visualizations span processor to print Honeycomb trace URLs
    .AddLocalVisualizations(serviceName, apikey)
    // configure OTLP exporter to send trace data to Honeycomb
    .AddHoneycombOtlpExporter(apikey)
    .Build();

var tracer = tracerProvider.GetTracer(serviceName);
var span = tracer.StartActiveSpan("doSomething");
span.SetAttribute("user_id", 123);

// Ensure all telemetry is exported before the application exits
tracerProvider.Shutdown();
