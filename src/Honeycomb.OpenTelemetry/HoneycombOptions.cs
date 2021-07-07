using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Collections.Generic;

namespace Honeycomb.OpenTelemetry
{
    public class HoneycombOptions
    {
        public const string DefaultEndpoint = "https://api.honeycomb.io:443";
        public const uint DefaultSampleRate = 1;
        
        private static readonly string DefaultServiceName = Assembly.GetEntryAssembly().GetName().Name;
        private static readonly string DefaultServiceVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        public string ApiKey { get; set; }
        public string Dataset { get; set; }
        public string Endpoint { get; set; } = DefaultEndpoint;
        public uint SampleRate { get; set; } = DefaultSampleRate;
        public string ServiceName { get; set; } = DefaultServiceName;
        public string ServiceVersion { get; set; } = DefaultServiceVersion;

        private static Dictionary<string, string> CommandLineSwitchMap = new Dictionary<string, string>
        {
            {"--honeycomb-apikey", "apikey"},
            {"--honeycomb-dataset", "dataset"},
            {"--honeycomb-endpoint", "endpoint"},
            {"--honeycomb-samplerate", "samplerate"},
            {"--service-name", "servicename"},
            {"--service-version", "serviceversion"}
        };

        public static HoneycombOptions FromArgs(params string[] args)
        {
            return new ConfigurationBuilder()
                .AddCommandLine(args, CommandLineSwitchMap)
                .Build()
                .Get<HoneycombOptions>();
        }

        public static HoneycombOptions FromConfiguration(IConfiguration configuration, params string[] args)
        {
            const string configurationKey = "Honeycomb";
            return new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .Build()
                .GetSection(configurationKey)
                .Get<HoneycombOptions>();
        }
    }
}
