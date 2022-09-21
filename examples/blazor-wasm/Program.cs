using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using blazor_wasm;
using OpenTelemetry.Trace;
using Honeycomb.OpenTelemetry;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddOpenTelemetryTracing(builder => builder.AddHoneycomb());

await builder.Build().RunAsync();
