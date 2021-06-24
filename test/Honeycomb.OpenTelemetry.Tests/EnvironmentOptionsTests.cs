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
                {"HONEYCOMB_DATASET", "my-dataset"},
                {"HONEYCOMB_API_ENDPOINT", "my-endpoint"},
                {"HONEYCOMB_SAMPLE_RATE", "10"},
                {"SERVICE_NAME", "my-service-name"},
                {"SERVICE_VERSION", "my-service-version"},
            };
            var options = new EnvironmentOptions(values);
            Assert.Equal("my-api-key", options.ApiKey);
            Assert.Equal("my-dataset", options.Dataset);
            Assert.Equal("my-endpoint", options.ApiEndpoint);
            Assert.Equal((uint) 10, options.SampleRate);
            Assert.Equal("my-service-name", options.ServiceName);
            Assert.Equal("my-service-version", options.ServiceVersion);
        }

        [Fact]
        public void Api_endpoint_returns_default_when_not_set()
        {
            var options = new EnvironmentOptions(new Dictionary<string, string>());
            Assert.Equal("https://api.honeycomb.io:443", options.ApiEndpoint);
        }

        [Fact]
        public void Sample_rate_returns_default_when_not_set()
        {
            var options = new EnvironmentOptions(new Dictionary<string, string>());
            Assert.Equal((uint) 1, options.SampleRate);
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