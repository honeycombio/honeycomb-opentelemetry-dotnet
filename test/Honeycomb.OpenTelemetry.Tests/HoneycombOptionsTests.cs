using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Xunit;

namespace Honeycomb.OpenTelemetry.Tests
{
    public class HoneycombOptionsHelperTests
    {
        [Fact]
        public void CanParseOptionsFromConfiguration()
        {
            var options =
                    new ConfigurationBuilder()
                        .AddJsonFile("appsettings.test.json")
                        .Build()
                        .GetSection(HoneycombOptions.ConfigSectionName)
                        .Get<HoneycombOptions>()
                ;

            Assert.Equal("my-apikey", options.ApiKey);
            Assert.Equal("my-traces-apikey", options.TracesApiKey);
            Assert.Equal("my-metrics-apikey", options.MetricsApiKey);
            Assert.Equal("my-dataset", options.Dataset);
            Assert.Equal("my-traces-dataset", options.TracesDataset);
            Assert.Equal("my-metrics-dataset", options.MetricsDataset);
            Assert.Equal((uint)5, options.SampleRate);
            Assert.Equal("my-endpoint", options.Endpoint);
            Assert.Equal("my-traces-endpoint", options.TracesEndpoint);
            Assert.Equal("my-metrics-endpoint", options.MetricsEndpoint);
            Assert.Equal("my-service", options.ServiceName);
            Assert.Equal("my-version", options.ServiceVersion);
            Assert.Equal(new List<string> { "meter1", "meter2" }, options.MeterNames);
            Assert.True(options.EnableLocalVisualizations);
            Assert.True(options.Debug);
        }

        [Fact]
        public void ApiKeyFallbacks()
        {
            var options = new HoneycombOptions { ApiKey = "mykey" };

            Assert.Equal("mykey", options.ApiKey);
            Assert.Equal("mykey", options.TracesApiKey);
            Assert.Equal("mykey", options.MetricsApiKey);

            options.TracesApiKey = "traces";
            options.MetricsApiKey = "metrics";

            Assert.Equal("mykey", options.ApiKey);
            Assert.Equal("traces", options.TracesApiKey);
            Assert.Equal("metrics", options.MetricsApiKey);
        }

        [Fact]
        public void DatasetFallbacks()
        {
            var options = new HoneycombOptions { Dataset = "dataset" };

            Assert.Equal("dataset", options.Dataset);
            Assert.Equal("dataset", options.TracesDataset);
            Assert.Null(options.MetricsDataset); // do not fall back metrics dataset

            options.TracesDataset = "traces";
            options.MetricsDataset = "metrics";

            Assert.Equal("dataset", options.Dataset);
            Assert.Equal("traces", options.TracesDataset);
            Assert.Equal("metrics", options.MetricsDataset);
        }

        [Fact]
        public void EndpointFallbacks()
        {
            var options = new HoneycombOptions { Endpoint = "endpoint" };

            Assert.Equal("endpoint", options.Endpoint);
            Assert.Equal("endpoint", options.TracesEndpoint);
            Assert.Equal("endpoint", options.MetricsEndpoint);

            options.TracesEndpoint = "traces";
            options.MetricsEndpoint = "metrics";

            Assert.Equal("endpoint", options.Endpoint);
            Assert.Equal("traces", options.TracesEndpoint);
            Assert.Equal("metrics", options.MetricsEndpoint);
        }

        [Fact]
        public void Legacy_key_length()
        {
            var options = new HoneycombOptions { ApiKey = "1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a" };
            Assert.True(options.IsTracesLegacyKey());
            Assert.True(options.IsMetricsLegacyKey());
        }
        [Fact]
        public void Not_legacy_key_length()
        {
            var options = new HoneycombOptions { ApiKey = "specialenvkey" };
            Assert.False(options.IsTracesLegacyKey());
            Assert.False(options.IsMetricsLegacyKey());
        }
    }
}