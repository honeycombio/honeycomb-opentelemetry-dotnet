using System;
using Xunit;

namespace Honeycomb.OpenTelemetry
{
    public class EnvironmentOptionsTests
    {
        [Fact]
        public void Can_get_options_from_env_vars()
        {
            Environment.SetEnvironmentVariable("HONEYCOMB_API_KEY", "my-api-key");
            Environment.SetEnvironmentVariable("HONEYCOMB_DATASET", "my-dataset");
            Environment.SetEnvironmentVariable("HONEYCOMB_API_ENDPOINT", "my-endpoint");
            Environment.SetEnvironmentVariable("HONEYCOMB_SAMPLE_RATE", "10");
            Environment.SetEnvironmentVariable("SERVICE_NAME", "my-service-name");
            Environment.SetEnvironmentVariable("SERVICE_VERSION", "my-service-version");

            var options = new EnvironmentOptions();
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
            Environment.SetEnvironmentVariable("HONEYCOMB_API_ENDPOINT", "");

            var options = new EnvironmentOptions();
            Assert.Equal("https://api.honeycomb.io:443", options.ApiEndpoint);
        }

        [Fact]
        public void Sample_rate_returns_default_when_not_set()
        {
            Environment.SetEnvironmentVariable("HONEYCOMB_SAMPLE_RATE", "");

            var options = new EnvironmentOptions();
            Assert.Equal((uint) 1, options.SampleRate);
        }

        [Fact]
        public void Sample_rate_returns_default_for_invalid_value()
        {
            Environment.SetEnvironmentVariable("HONEYCOMB_SAMPLE_RATE", "invalid");

            var options = new EnvironmentOptions();
            Assert.Equal((uint) 1, options.SampleRate);
        }
    }
}