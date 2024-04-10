using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var serviceName = Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME");
var apiKey = Environment.GetEnvironmentVariable("HONEYCOMB_API_KEY");

// Setup OpenTelemetry Tracing
builder.Services.AddOpenTelemetry().WithTracing(otelBuilder =>
    otelBuilder
        // configure OTel SDK to capture activity for your service name
        .AddSource(serviceName)
        // add baggage span processor to copy baggage items from parent to child spans
        .AddBaggageSpanProcessor()
        // add deterministic sampler to sample 100% of traces
        .AddDeterministicSampler(1)
        // add local visualizations that prints Honeycomb URLs for common traces
        .AddLocalVisualizations(serviceName, apiKey)
        // configure OTel SDK to send trace data to Honeycomb
        .AddHoneycombOtlpExporter(apiKey)
        // enable aspnetcore instrumentation
        .AddAspNetCoreInstrumentationWithBaggage()
);

// Register Tracer so it can be injected into other components (eg Controllers)
builder.Services.AddSingleton(TracerProvider.Default.GetTracer(serviceName));

var app = builder.Build();

app.MapControllers();
await app.RunAsync();