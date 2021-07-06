using OpenTelemetry.Trace;
using Honeycomb.OpenTelemetry;
using System.Text.Json;

using Microsoft.Extensions.Configuration;

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
                Dataset = "{datast}"
            };

            // configure OpenTelemetry SDK to send data to Honeycomb
            var provider = OpenTelemetry.Sdk.CreateTracerProviderBuilder()
                .UseHoneycomb(options)
                .Build();
            
            // create span to describe some application logic
            var tracer = provider.GetTracer(options.ServiceName);
            using (var span = tracer.StartActiveSpan("doSomething"))
            {
                span.SetAttribute("user_id", 123);
            }   

            // dispose of provider to ensure spans are fluhsed and resoruces are cleaned up
            provider.Dispose();
        }
    }
}
