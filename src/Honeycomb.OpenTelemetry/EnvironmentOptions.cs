using System.Collections;
using System.Collections.Generic;

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
        private const string DebugKey = "DEBUG";
        private const uint DefaultSampleRate = 1;
        private const string DefaultApiEndpoint = "https://api.honeycomb.io:443";
        private readonly IDictionary _environmentService;

        internal EnvironmentOptions() => new EnvironmentOptions(new Dictionary<string, string>());
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
        internal bool Debug => bool.TryParse(GetEnvironmentVariable(DebugKey), out var debug) ? debug : false;
        internal uint SampleRate => uint.TryParse(GetEnvironmentVariable(SampleRateKey), out var sampleRate) ? sampleRate : DefaultSampleRate;

        internal void SetOptionsFromEnvironmentIfTheyExist(HoneycombOptions options)
        {
            if (!string.IsNullOrWhiteSpace(ApiKey))
            {
                options.ApiKey = ApiKey;
            }

            if (!string.IsNullOrWhiteSpace(TracesApiKey))
            {
                options.TracesApiKey = TracesApiKey;
            }

            if (!string.IsNullOrWhiteSpace(MetricsApiKey))
            {
                options.MetricsApiKey = MetricsApiKey;
            }

            if (!string.IsNullOrWhiteSpace(Dataset))
            {
                options.Dataset = Dataset;
            }

            if (!string.IsNullOrWhiteSpace(TracesDataset))
            {
                options.TracesDataset = TracesDataset;
            }

            if (!string.IsNullOrWhiteSpace(MetricsDataset))
            {
                options.MetricsDataset = MetricsDataset;
            }

            if (!string.IsNullOrWhiteSpace(ApiEndpoint))
            {
                options.Endpoint = ApiEndpoint;
            }

            if (!string.IsNullOrWhiteSpace(TracesEndpoint))
            {
                options.TracesEndpoint = TracesEndpoint;
            }

            if (!string.IsNullOrWhiteSpace(MetricsEndpoint))
            {
                options.MetricsEndpoint = MetricsEndpoint;
            }

            if (!string.IsNullOrWhiteSpace(ServiceName))
            {
                options.ServiceName = ServiceName;
            }

            if (!string.IsNullOrWhiteSpace(ServiceVersion))
            {
                options.ServiceVersion = ServiceVersion;
            }

            if (bool.TryParse(GetEnvironmentVariable(EnableLocalVisualizationsKey), out var enableLocalVisualizations))
            {
                options.EnableLocalVisualizations = enableLocalVisualizations;
            }

            if (bool.TryParse(GetEnvironmentVariable(DebugKey), out var debug))
            {
                options.Debug = debug;
            }

            if (uint.TryParse(GetEnvironmentVariable(SampleRateKey), out var sampleRate))
            {
                options.SampleRate = sampleRate;
            }
        }

        private string GetEnvironmentVariable(string key, string defaultValue = "")
        {
            var value = _environmentService[key];
            if (value is string str && !string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            return defaultValue;
        }

        internal static string GetErrorMessage(string humanKey, string key)
        {
            return ($"Missing {humanKey}. Specify {key} environment variable, or the associated property in appsettings.json or the command line");
        }
    }
}