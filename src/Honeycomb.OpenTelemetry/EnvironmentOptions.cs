using System.Collections;

namespace Honeycomb.OpenTelemetry
{
    public class HoneycombOptions
    {
        public string ApiKey { get; set; }
        public string Dataset { get; set; }
        public string ApiEndpoint { get; set; }
        public string ServiceName { get; set; }
        public string ServiceVersion { get; set; }
        public uint SampleRate { get; set; }
    }

    internal class EnvironmentOptions
    {
        private const string ApiKeyKey = "HONEYCOMB_API_KEY";
        private const string DatasetKey = "HONEYCOMB_DATASET";
        private const string ApiEndpointKey = "HONEYCOMB_API_ENDPOINT";
        private const string SampleRateKey = "HONEYCOMB_SAMPLE_RATE";
        private const string ServiceNameKey = "SERVICE_NAME";
        private const string ServiceVersionKey = "SERVICE_VERSION";
        private const uint DefaultSampleRate = 1;
        private const string DefaultApiEndpoint = "https://api.honeycomb.io:443";
        private readonly IDictionary _environmentService;

        internal EnvironmentOptions(IDictionary service)
        {
            _environmentService = service;
        }

        internal string ApiKey { get => GetEnvironmentVariable(ApiKeyKey); }
        internal string Dataset  { get => GetEnvironmentVariable(DatasetKey); }
        internal string ApiEndpoint { get => GetEnvironmentVariable(ApiEndpointKey, DefaultApiEndpoint); }
        internal string ServiceName { get => GetEnvironmentVariable(ServiceNameKey); }
        internal string ServiceVersion { get => GetEnvironmentVariable(ServiceVersionKey); }
        internal uint SampleRate { get => uint.TryParse(GetEnvironmentVariable(SampleRateKey), out var sampleRate) ? sampleRate : DefaultSampleRate; }

        private string GetEnvironmentVariable(string key, string defaultValue = "")
        {
            var value = _environmentService[key];
            if (value is string && !string.IsNullOrWhiteSpace((string) value))
            {
                return (string) value;
            }

            return defaultValue;
        }
    }
}