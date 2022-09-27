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
        private const string EnableLocalVisualizationsKey = "HONEYCOMB_ENABLE_LOCAL_VISUALIZATIONS";
        private const string DebugKey = "DEBUG";
        private const string OtelExporterOtlpProtocolKey = "OTEL_EXPORTER_OTLP_PROTOCOL";
        private const uint DefaultSampleRate = 1;
        private const string DefaultApiEndpoint = "https://api.honeycomb.io:443";
        private readonly IDictionary _environmentService;

        internal EnvironmentOptions() => new EnvironmentOptions(new Dictionary<string, string>());
        internal EnvironmentOptions(IDictionary service)
        {
            _environmentService = service;
        }

        internal string ApiKey => GetEnvironmentVariable(ApiKeyKey);
        internal string TracesApiKey => GetEnvironmentVariable(TracesApiKeyKey);
        internal string MetricsApiKey => GetEnvironmentVariable(MetricsApiKeyKey);
        internal string Dataset => GetEnvironmentVariable(DatasetKey);
        internal string TracesDataset => GetEnvironmentVariable(TracesDatasetKey);
        internal string MetricsDataset => GetEnvironmentVariable(MetricsDatasetKey);
        internal string ApiEndpoint => GetEnvironmentVariable(ApiEndpointKey);
        internal string TracesEndpoint => GetEnvironmentVariable(TracesEndpointKey);
        internal string MetricsEndpoint => GetEnvironmentVariable(MetricsEndpointKey);
        internal string ServiceName => GetEnvironmentVariable(ServiceNameKey);
        internal string ServiceVersion => GetEnvironmentVariable(ServiceVersionKey);
        internal string EnableLocalVisualizationsValue => GetEnvironmentVariable(EnableLocalVisualizationsKey);
        internal bool EnableLocalVisualizations => bool.TryParse(EnableLocalVisualizationsValue, out var enableLocalVisualizations) ? enableLocalVisualizations : false;
        internal string DebugValue => GetEnvironmentVariable(DebugKey);
        internal bool Debug => bool.TryParse(DebugValue, out var debug) ? debug : false;
        internal string SampleRateValue => GetEnvironmentVariable(SampleRateKey);
        internal uint SampleRate => uint.TryParse(SampleRateValue, out var sampleRate) ? sampleRate : DefaultSampleRate;
        internal string OtelExporterOtlpProtocol => GetEnvironmentVariable(OtelExporterOtlpProtocolKey);
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