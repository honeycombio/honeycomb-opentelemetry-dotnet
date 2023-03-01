using OpenTelemetry.Trace;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var redis = ConnectionMultiplexer.Connect(
    new ConfigurationOptions
    {
        EndPoints = { "localhost:6379" },
        AbortOnConnectFail = false, // allow for reconnects if redis is not available
    }
);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

var honeycombOptions = builder.Configuration.GetHoneycombOptions();

builder.Services.AddOpenTelemetry().WithTracing(otelBuilder =>
    otelBuilder
        .AddHoneycomb(honeycombOptions)
        .AddAspNetCoreInstrumentationWithBaggage()
        .AddRedisInstrumentation(redis)
);

builder.Services.AddSingleton(TracerProvider.Default.GetTracer(honeycombOptions.ServiceName));

var app = builder.Build();
app.MapControllers();
await app.RunAsync();