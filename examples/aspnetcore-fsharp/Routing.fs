module Routing

open System
open System.Diagnostics.Metrics

open Microsoft.AspNetCore.Http

open Giraffe
open Giraffe.EndpointRouting
open OpenTelemetry.Trace

type WeatherForecast =
    { Date: DateTime
      TemperatureC: int
      Summary: string }

let private createForecast index (rng: Random) (weatherSummaries: string[]) =
    { Date = DateTime.Now.AddDays(index)
      TemperatureC = rng.Next(-20,55)
      Summary = weatherSummaries[rng.Next(weatherSummaries.Length)] }

let weatherSummaries =
    [|
        "Freezing"
        "Bracing"
        "Chilly"
        "Cool"
        "Mild"
        "Warm"
        "Balmy"
        "Hot"
        "Sweltering"
        "Scorching"
    |]

let private weatherForecastHandler (tracer: Tracer) (sheepCounter: Counter<int>) =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        use forecastSpan = tracer.StartActiveSpan("app.weatherForecast")

        let rng = Random()
        let forecast =
            [|
                for index in 0..4 ->
                    createForecast index rng weatherSummaries
            |]

        forecastSpan.SetAttribute("app.weatherForecast.days", forecast.Length) |> ignore

        sheepCounter.Add(1)

        json forecast next ctx
        
let endpoints (tracer: Tracer) (meter: Meter) =
    let sheepCounter = meter.CreateCounter("app.sheep")
    [
        GET [
            route  "/" (text "Hello World")
            route "/weatherforecast" (weatherForecastHandler tracer sheepCounter)
        ]
    ]