using System.Reflection;

namespace Honeycomb.OpenTelemetry
{
    public class HoneycombOptions
    {
        private const string DefualtEndpoint = "https://api.honeycomb.io:443";
        private const uint DefaultSampleRate = 1;
        private static readonly string DefaultServiceName = Assembly.GetEntryAssembly().GetName().Name;
        private static readonly string DefaultServiceVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;


        public string ApiKey { get; set; }
        public string Dataset { get; set; }
        public string ServiceName { get; set; } = DefaultServiceName;
        public string ServiceVersion { get; set; } = DefaultServiceVersion;
        public string Endpoint { get; set; } = DefualtEndpoint;
        public uint SampleRate { get; set; } = DefaultSampleRate;
    }
}
