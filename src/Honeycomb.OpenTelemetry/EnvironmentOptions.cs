using System.Collections;

namespace Honeycomb.OpenTelemetry
{
    internal class EnvironmentOptions
    {
        private const string ApiKeyKey = "HONEYCOMB_API_KEY";
        private const string TracesApiKeyKey = "HONEYCOMB_TRACES_API_KEY";
        private const string MetricsApiKeyKey = "HONEYCOMB_METRICS_API_KEY";
        private const string DatasetKey = "HONEYCOMB_DATASET";
        private const string TracesDatasetKey = "HONEYCOMB_TRACES_DATASET";
        private const string MetricsDatasetKey = "HONEYCOMB_METRICS_DATASET";
        private const string ApiEndpointKey = "HONEYCOMB_API_ENDPOINT";
        private const string TracesEndpointKey = "HONEYCOMB_TRACES_ENDPOINT";
        private const string MetricsEndpointKey = "HONEYCOMB_METRICS_ENDPOINT";
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
        internal string TracesApiKey { get => GetEnvironmentVariable(TracesApiKeyKey, ApiKey); }
        internal string MetricsApiKey { get => GetEnvironmentVariable(MetricsApiKeyKey, ApiKey); }
        internal string Dataset { get => GetEnvironmentVariable(DatasetKey); }
        internal string TracesDataset { get => GetEnvironmentVariable(TracesDatasetKey, Dataset); }
        internal string MetricsDataset { get => GetEnvironmentVariable(MetricsDatasetKey); }
        internal string ApiEndpoint { get => GetEnvironmentVariable(ApiEndpointKey, DefaultApiEndpoint); }
        internal string TracesEndpoint { get => GetEnvironmentVariable(TracesEndpointKey, ApiEndpoint); }
        internal string MetricsEndpoint { get => GetEnvironmentVariable(MetricsEndpointKey, ApiEndpoint); }
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

        public static string getErrorMessage(string humanKey, string key) {
            return ($"Missing {humanKey}. Specify {key} environment variable, or the associated property in appsettings.json or the command line");
        }
    }
}