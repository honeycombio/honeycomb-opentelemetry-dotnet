using OpenTelemetry.Trace;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var honeycombOptions = builder.Configuration.GetHoneycombOptions();

builder.Services.AddOpenTelemetryTracing(otelBuilder =>
    otelBuilder
        .AddHoneycomb(honeycombOptions)
        .AddAspNetCoreInstrumentationWithBaggage()
);

builder.Services.AddSingleton(TracerProvider.Default.GetTracer(honeycombOptions.ServiceName));

// (optional metrics setup)
// meter name used here must be configured in the OpenTelemetry SDK
// service name is configured by default
// you may configure additional meter names using the Honeycomb options
var meter = new Meter(honeycombOptions.MetricsDataset);
builder.Services.AddSingleton(meter);

var app = builder.Build();

app.MapControllers();
await app.RunAsync();