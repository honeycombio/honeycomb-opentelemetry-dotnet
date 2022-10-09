open System
open System.Diagnostics.Metrics

open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration

open Giraffe
open Giraffe.EndpointRouting
open OpenTelemetry.Trace
open OpenTelemetry.Metrics
open Honeycomb.OpenTelemetry

let configureServices (services: IServiceCollection) (honeycombOptions: HoneycombOptions) =
    services
        .AddRouting()
        .AddGiraffe()
        .AddOpenTelemetryTracing(fun otelBuilder ->
            otelBuilder
                .AddHoneycomb(honeycombOptions)
                .AddAspNetCoreInstrumentationWithBaggage()
            |> ignore)
        .AddOpenTelemetryMetrics(fun otelBuilder ->
            otelBuilder.AddHoneycomb(honeycombOptions)
            |> ignore)
    |> ignore

let configureApp (appBuilder: IApplicationBuilder) (tracer: Tracer) (meter: Meter) =
    appBuilder
        .UseRouting()
        .UseGiraffe(Routing.endpoints tracer meter)
    |> ignore

let args = Environment.GetCommandLineArgs()
let builder = WebApplication.CreateBuilder(args)
let honeycombOptions = builder.Configuration.GetHoneycombOptions();

configureServices builder.Services honeycombOptions

let tracer = TracerProvider.Default.GetTracer(honeycombOptions.ServiceName)
let meter = new Meter(honeycombOptions.MetricsDataset)

let app = builder.Build()
configureApp app tracer meter

app.Run()