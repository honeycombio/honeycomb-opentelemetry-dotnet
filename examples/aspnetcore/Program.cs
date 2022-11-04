using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var serviceName = builder.Configuration.GetValue<string>("Honeycomb:ServiceName");
var apikey = builder.Configuration.GetValue<string>("Honeycomb:ApiKey");

var honeycombOptions = builder.Configuration.GetHoneycombOptions();

// Setup OpenTelemetry Tracing
builder.Services.AddOpenTelemetryTracing(otelBuilder =>
    otelBuilder
        .AddHoneycomb(honeycombOptions)
        .AddAspNetCoreInstrumentationWithBaggage()
);

// Register Tracer so it can be injected into other components (eg Controllers)
builder.Services.AddSingleton(TracerProvider.Default.GetTracer(honeycombOptions.ServiceName));

// Setup OpenTelemetry Metrics
builder.Services.AddOpenTelemetryMetrics(otelBuilder =>
    otelBuilder.AddHoneycomb(honeycombOptions)
);

// Register Meter so it can be injected into other components (eg controllers)
builder.Services.AddSingleton(new Meter(honeycombOptions.MetricsDataset));

builder.Logging.AddOpenTelemetry(options =>
{
    options.ConfigureResource(r => r.AddService(serviceName));
    options.AddOtlpExporter(otlpOptions =>
    {
        otlpOptions.Endpoint = new Uri("https://api.honeycomb.io:443");
        otlpOptions.Headers = $"x-honeycomb-team={apikey}";
    });
});

var app = builder.Build();

app.MapControllers();
await app.RunAsync();