using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using blazor_wasm;
using OpenTelemetry.Trace;
using Honeycomb.OpenTelemetry;

const string appName = "blazor-example";

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddOpenTelemetryTracing(builder =>
{
    builder.AddHoneycomb(options =>
    {
        options.ServiceName = appName;
        options.ApiKey = "{apikey}";
        options.Debug = true;
    });
    builder.AddConsoleExporter();
});
builder.Services.AddSingleton(TracerProvider.Default.GetTracer(appName));

await builder.Build().RunAsync();
