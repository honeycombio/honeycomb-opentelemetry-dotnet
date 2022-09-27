using Honeycomb.OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var honeycombOptions =
    builder.Configuration.GetSection(HoneycombOptions.ConfigSectionName)
        .Get<HoneycombOptions>();

// Setup OpenTelemetry Tracing
builder.Services.AddOpenTelemetryTracing(otelBuilder =>
    otelBuilder
        .AddHoneycomb(builder.Configuration)
        .AddAspNetCoreInstrumentationWithBaggage()
);

// Register Tracer so it can be injected into other components (eg Controllers)
builder.Services.AddSingleton(TracerProvider.Default.GetTracer(honeycombOptions.ServiceName));

// Setup OpenTelemetry Metrics
builder.Services.AddOpenTelemetryMetrics(otelBuilder =>
    otelBuilder.AddHoneycomb(honeycombOptions)
);

// Register Meter so it can be injected into other components (eg controllers)
builder.Services.AddSingleton(new Meter(honeycombOptions.ServiceName));

var app = builder.Build();

app.MapControllers();
await app.RunAsync();