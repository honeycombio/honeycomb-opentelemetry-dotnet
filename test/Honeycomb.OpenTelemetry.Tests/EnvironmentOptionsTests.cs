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
                {"SERVICE_NAME", "my-service-name"},
                {"SERVICE_VERSION", "my-service-version"},
                {"ENABLE_LOCAL_VISUALIZATIONS", "true" }
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
            Assert.Equal((uint) 10, options.SampleRate);
            Assert.Equal("my-service-name", options.ServiceName);
            Assert.Equal("my-service-version", options.ServiceVersion);
            Assert.True(options.EnableLocalVisualizations);
        }

        [Fact]
        public void Optional_args_fall_back_to_defaults()
        {
            var options = new EnvironmentOptions(new Dictionary<string, string>());            
            Assert.Equal(options.ApiKey, options.TracesApiKey);
            Assert.Equal(options.ApiKey, options.MetricsApiKey);
            Assert.Equal(options.Dataset, options.TracesDataset);
            Assert.Empty(options.MetricsDataset);
            Assert.Equal("https://api.honeycomb.io:443", options.ApiEndpoint);
            Assert.Equal(options.ApiEndpoint, options.TracesEndpoint);
            Assert.Equal(options.ApiEndpoint, options.MetricsEndpoint);
            Assert.Equal((uint) 1, options.SampleRate);
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
            Assert.Equal((uint) 1, options.SampleRate);
        }
    }
}