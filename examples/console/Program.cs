using OpenTelemetry.Trace;
using Honeycomb.OpenTelemetry;

namespace console
{
    class Program
    {
        static void Main(string[] args)
        {
            const string serviceName = "my-app";

            // configure OpenTelemetry SDK to send data to Honeycomb
            // NOTE: the tracer provider should be a long-lived resource, and disposed
            // at the end of your app lifecycle to ensure all telemetry is exported
            using var provider = OpenTelemetry.Sdk.CreateTracerProviderBuilder()
                .AddHoneycomb(options => {
                    options.ServiceName = serviceName;
                    options.ApiKey = "{apikey}";
                    options.Dataset = "{dataset}";
                })
                .Build();
            
            // get an instance of a tracer that can be used to create spans
            var tracer = provider.GetTracer(serviceName);

            // create span to describe some application logic
            using var span = tracer.StartActiveSpan("doSomething");
            span.SetAttribute("user_id", 123);
        }
    }
}
