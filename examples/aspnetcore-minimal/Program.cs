using OpenTelemetry.Trace;
using Honeycomb.OpenTelemetry;

// Boilerplate to configure the app and get a tracer
var builder = WebApplication.CreateBuilder(args);
var options = HoneycombOptions.FromConfiguration(builder.Configuration);
builder.Services.AddHoneycomb(options);
var app = builder.Build();
var tracer = TracerProvider.Default.GetTracer(options.ServiceName);

var weatherSummaries = new[]
{
    "Freezing", "Bracing", "Chilly", 
    "Cool", "Mild", "Warm", "Balmy", 
    "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async () =>
{
    using var span = tracer.StartActiveSpan("sleep");
    span.SetAttribute("delay_ms", 100);

    await Task.Delay(100);

    var rng = new Random();
    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    (
        Date: DateTimeOffset.Now.AddDays(index),
        TemperatureC: rng.Next(-20, 55),
        Summary: weatherSummaries[rng.Next(weatherSummaries.Length)]
    ));
});

app.Run();

record WeatherForecast(DateTimeOffset Date, int TemperatureC, string Summary);