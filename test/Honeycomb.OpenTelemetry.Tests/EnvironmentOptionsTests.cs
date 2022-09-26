using System.Collections.Generic;
using Xunit;

namespace Honeycomb.OpenTelemetry
{
    public class EnvironmentOptionsTests
    {
        [Fact]
        public void Can_get_options_from_env_vars()
        {
            var values = new Dictionary<string, string>
            {
                {"HONEYCOMB_API_KEY", "my-api-key"},
                {"HONEYCOMB_TRACES_API_KEY", "my-traces-api-key"},
                {"HONEYCOMB_METRICS_API_KEY", "my-metrics-api-key"},
                {"HONEYCOMB_DATASET", "my-dataset"},
                {"HONEYCOMB_TRACES_DATASET", "my-traces-dataset"},
                {"HONEYCOMB_METRICS_DATASET", "my-metrics-dataset"},
                {"HONEYCOMB_API_ENDPOINT", "my-endpoint"},
                {"HONEYCOMB_TRACES_ENDPOINT", "my-traces-endpoint"},
                {"HONEYCOMB_METRICS_ENDPOINT", "my-metrics-endpoint"},
                {"HONEYCOMB_SAMPLE_RATE", "10"},
                {"OTEL_SERVICE_NAME", "my-service-name"},
                {"SERVICE_VERSION", "my-service-version"},
                {"HONEYCOMB_ENABLE_LOCAL_VISUALIZATIONS", "true" },
                {"DEBUG", "true"}
            };
            var options = new EnvironmentOptions(values);
            Assert.Equal("my-api-key", options.ApiKey);
            Assert.Equal("my-traces-api-key", options.TracesApiKey);
            Assert.Equal("my-metrics-api-key", options.MetricsApiKey);
            Assert.Equal("my-dataset", options.Dataset);
            Assert.Equal("my-traces-dataset", options.TracesDataset);
            Assert.Equal("my-metrics-dataset", options.MetricsDataset);
            Assert.Equal("my-endpoint", options.ApiEndpoint);
            Assert.Equal("my-traces-endpoint", options.TracesEndpoint);
            Assert.Equal("my-metrics-endpoint", options.MetricsEndpoint);
            Assert.Equal((uint)10, options.SampleRate);
            Assert.Equal("my-service-name", options.ServiceName);
            Assert.Equal("my-service-version", options.ServiceVersion);
            Assert.True(options.EnableLocalVisualizations);
            Assert.True(options.Debug);
        }

        [Fact]
        public void EnvironmentOptionsCanOverrideHoneycombOptions()
        {
            var honeycombOptions = new HoneycombOptions
            {
                ApiKey = "my-api-key",
                TracesApiKey = "my-traces-api-key",
                MetricsApiKey = "my-metrics-api-key",
                Dataset = "my-dataset",
                TracesDataset = "my-traces-dataset",
                MetricsDataset = "my-metrics-dataset",
                Endpoint = "my-endpoint",
                TracesEndpoint = "my-traces-endpoint",
                MetricsEndpoint = "my-metrics-endpoint",
                SampleRate = 2,
                ServiceName = "my-service-name",
                ServiceVersion = "my-service-version",
                EnableLocalVisualizations = false,
                Debug = false
            };

            var values = new Dictionary<string, string>
            {
                {"HONEYCOMB_API_KEY", "my-env-api-key"},
                {"HONEYCOMB_TRACES_API_KEY", "my-env-traces-api-key"},
                {"HONEYCOMB_METRICS_API_KEY", "my-env-metrics-api-key"},
                {"HONEYCOMB_DATASET", "my-env-dataset"},
                {"HONEYCOMB_TRACES_DATASET", "my-env-traces-dataset"},
                {"HONEYCOMB_METRICS_DATASET", "my-env-metrics-dataset"},
                {"HONEYCOMB_API_ENDPOINT", "my-env-endpoint"},
                {"HONEYCOMB_TRACES_ENDPOINT", "my-env-traces-endpoint"},
                {"HONEYCOMB_METRICS_ENDPOINT", "my-env-metrics-endpoint"},
                {"HONEYCOMB_SAMPLE_RATE", "10"},
                {"OTEL_SERVICE_NAME", "my-env-service-name"},
                {"SERVICE_VERSION", "my-env-service-version"},
                {"HONEYCOMB_ENABLE_LOCAL_VISUALIZATIONS", "true" },
                {"DEBUG", "true"}
            };
            var options = new EnvironmentOptions(values);
            Assert.Equal("my-env-api-key", options.ApiKey);
            Assert.Equal("my-env-traces-api-key", options.TracesApiKey);
            Assert.Equal("my-env-metrics-api-key", options.MetricsApiKey);
            Assert.Equal("my-env-dataset", options.Dataset);
            Assert.Equal("my-env-traces-dataset", options.TracesDataset);
            Assert.Equal("my-env-metrics-dataset", options.MetricsDataset);
            Assert.Equal("my-env-endpoint", options.ApiEndpoint);
            Assert.Equal("my-env-traces-endpoint", options.TracesEndpoint);
            Assert.Equal("my-env-metrics-endpoint", options.MetricsEndpoint);
            Assert.Equal((uint)10, options.SampleRate);
            Assert.Equal("my-env-service-name", options.ServiceName);
            Assert.Equal("my-env-service-version", options.ServiceVersion);
            Assert.True(options.EnableLocalVisualizations);
            Assert.True(options.Debug);
        }

        [Fact]
        public void Optional_args_fall_back_to_defaults()
        {
            var options = new EnvironmentOptions(new Dictionary<string, string>());
            Assert.Equal((uint)1, options.SampleRate);
            Assert.False(options.EnableLocalVisualizations);
        }

        [Fact]
        public void Sample_rate_returns_default_for_invalid_value()
        {
            var values = new Dictionary<string, string>
            {
                {"HONEYCOMB_SAMPLE_RATE", "invalid"}
            };
            var options = new EnvironmentOptions(values);
            Assert.Equal((uint)1, options.SampleRate);
        }
    }
}