using OpenTelemetry.Trace;
using Honeycomb.OpenTelemetry;
using OpenTelemetry.Metrics;
using System.Diagnostics.Metrics;

namespace console
{
    class Program
    {
        static void Main(string[] args)
        {
            // configure HoneycombOptions
            var options = new HoneycombOptions
            {
                ServiceName = "my-app",
                ApiKey = "{apikey}",
                TracesDataset = "{traces-dataset}",
                MetricsDataset = "{metrics-dataset}" // optional
            };

            // ------------ TRACES ------------

            // configure OpenTelemetry SDK to send trace data to Honeycomb
            // NOTE: the tracer provider should be a long-lived resource, and disposed
            // at the end of your app lifecycle to ensure all telemetry is exported
            using var tracerProvider = OpenTelemetry.Sdk.CreateTracerProviderBuilder()
                .AddHoneycomb(options)
                .Build();

            // get an instance of a tracer that can be used to create spans
            var tracer = tracerProvider.GetTracer(options.ServiceName);

            // create span to describe some application logic
            using var span = tracer.StartActiveSpan("doSomething");
            span.SetAttribute("user_id", 123);

            // ------------ METRICS (optional) ------------

            // configure OpenTelemetry SDK to send metric data to Honeycomb
            using var meterProvider = OpenTelemetry.Sdk.CreateMeterProviderBuilder()
                .AddHoneycomb(options)
                .Build();

            // meter name used here must be configured in the OpenTelemetry SDK
            // service name is configured by default
            // you may configure additional meter names using the Honeycomb options
            Meter meter = new Meter(options.ServiceName);
            Counter<int> counter = meter.CreateCounter<int>("sheep");
            counter.Add(1);
        }
    }
}