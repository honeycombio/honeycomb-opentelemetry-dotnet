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
        private const string ServiceNameKey = "OTEL_SERVICE_NAME";
        private const string ServiceVersionKey = "SERVICE_VERSION";
        private const string EnableLocalVisualizationsKey = "ENABLE_LOCAL_VISUALIZATIONS";
        private const uint DefaultSampleRate = 1;
        private const string DefaultApiEndpoint = "https://api.honeycomb.io:443";
        private readonly IDictionary _environmentService;

        internal EnvironmentOptions(IDictionary service)
        {
            _environmentService = service;
        }

        internal string ApiKey => GetEnvironmentVariable(ApiKeyKey);
        internal string TracesApiKey => GetEnvironmentVariable(TracesApiKeyKey, ApiKey);
        internal string MetricsApiKey => GetEnvironmentVariable(MetricsApiKeyKey, ApiKey);
        internal string Dataset => GetEnvironmentVariable(DatasetKey);
        internal string TracesDataset => GetEnvironmentVariable(TracesDatasetKey, Dataset);
        internal string MetricsDataset => GetEnvironmentVariable(MetricsDatasetKey);
        internal string ApiEndpoint => GetEnvironmentVariable(ApiEndpointKey, DefaultApiEndpoint);
        internal string TracesEndpoint => GetEnvironmentVariable(TracesEndpointKey, ApiEndpoint);
        internal string MetricsEndpoint => GetEnvironmentVariable(MetricsEndpointKey, ApiEndpoint);
        internal string ServiceName => GetEnvironmentVariable(ServiceNameKey);
        internal string ServiceVersion => GetEnvironmentVariable(ServiceVersionKey);
        internal bool EnableLocalVisualizations => bool.TryParse(GetEnvironmentVariable(EnableLocalVisualizationsKey), out var enableLocalVisualizations) ? enableLocalVisualizations : false;
        internal uint SampleRate => uint.TryParse(GetEnvironmentVariable(SampleRateKey), out var sampleRate) ? sampleRate : DefaultSampleRate;

        private string GetEnvironmentVariable(string key, string defaultValue = "")
        {
            var value = _environmentService[key];
            if (value is string str && !string.IsNullOrWhiteSpace(str))
            {
                return (string)value;
            }

            return defaultValue;
        }

        internal static string GetErrorMessage(string humanKey, string key)
        {
            return ($"Missing {humanKey}. Specify {key} environment variable, or the associated property in appsettings.json or the command line");
        }
    }
}