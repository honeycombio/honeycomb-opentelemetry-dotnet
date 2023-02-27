open System
open System.Diagnostics.Metrics

open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration

open Giraffe
open Giraffe.EndpointRouting

open OpenTelemetry
open OpenTelemetry.Trace
open OpenTelemetry.Metrics
open Honeycomb.OpenTelemetry

let configureServices (services: IServiceCollection) (honeycombOptions: HoneycombOptions) =
    services
        .AddRouting()
        .AddGiraffe()
        .AddOpenTelemetry()
        .WithTracing(fun otelBuilder ->
            otelBuilder
                .AddHoneycomb(honeycombOptions)
                .AddAspNetCoreInstrumentationWithBaggage()
            |> ignore)
        .WithMetrics(fun otelBuilder ->
            otelBuilder.AddHoneycomb(honeycombOptions)
            |> ignore)
    |> ignore

let configureApp (appBuilder: IApplicationBuilder) (honeycombOptions: HoneycombOptions) =
    let tracer = TracerProvider.Default.GetTracer(honeycombOptions.ServiceName)
    let meter = new Meter(honeycombOptions.MetricsDataset)
    appBuilder
        .UseRouting()
        .UseGiraffe(Routing.endpoints tracer meter)
    |> ignore

let args = Environment.GetCommandLineArgs()
let builder = WebApplication.CreateBuilder(args)
let honeycombOptions = builder.Configuration.GetHoneycombOptions();

configureServices builder.Services honeycombOptions

let app = builder.Build()
configureApp app honeycombOptions

app.Run()