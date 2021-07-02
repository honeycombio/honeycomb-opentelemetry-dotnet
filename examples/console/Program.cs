using OpenTelemetry.Trace;
using Honeycomb.OpenTelemetry;

namespace console
{
    class Program
    {
        static void Main(string[] args)
        {
            const string AppName = "my-app";

            // configure OpenTelemetry SDK
            var provider = OpenTelemetry.Sdk.CreateTracerProviderBuilder()
                .UseHoneycomb(options => {
                    options.ServiceName = AppName;
                    options.ApiKey = "{api-key}";
                    options.Dataset = "{dataset}";
                })
                .Build();
            
            // create span to describe some application logic
            var tracer = provider.GetTracer(AppName);
            using (var span = tracer.StartActiveSpan("doSomething"))
            {
                span.SetAttribute("user_id", 123);
            }   

            // dispose of provider to ensure spans are fluhsed and resoruces are cleaned up
            provider.Dispose();
        }
    }
}
