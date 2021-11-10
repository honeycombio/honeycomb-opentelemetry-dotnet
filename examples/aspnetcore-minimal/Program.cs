using System.Security.Cryptography;
using OpenTelemetry.Trace;
using Honeycomb.OpenTelemetry;

// Boilerplate to configure the app and get a tracer
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHoneycomb(builder.Configuration);

var weatherSummaries = new[]
{
    "Freezing", "Bracing", "Chilly", 
    "Cool", "Mild", "Warm", "Balmy", 
    "Hot", "Sweltering", "Scorching"
};

var app = builder.Build();
app.MapGet("/weatherforecast", async (Tracer tracer) =>
{
    using var span = tracer.StartActiveSpan("sleep");
    span.SetAttribute("custom.delay", 100);

    await Task.Delay(100);

    var randomTemp = RandomNumberGenerator.GetInt32(-20, 55);
    var randomSumary = weatherSummaries[RandomNumberGenerator.GetInt32(weatherSummaries.Length)];

    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    (
        Date: DateTimeOffset.Now.AddDays(index),
        TemperatureC: randomTemp,
        Summary: randomSumary
    ));
});

app.Run();

record WeatherForecast(DateTimeOffset Date, int TemperatureC, string Summary);