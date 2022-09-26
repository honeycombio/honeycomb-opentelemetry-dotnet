using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Xunit;

namespace Honeycomb.OpenTelemetry.Tests
{
    public class HoneycombOptionsHelperTests
    {
        [Fact]
<<<<<<< HEAD
=======
        public void CanParseOptionsFromSpacedCommandLineArgs()
        {
            var options = HoneycombOptions.FromArgs(
                "--honeycomb-apikey", "my-apikey",
                "--honeycomb-traces-apikey", "my-traces-apikey",
                "--honeycomb-metrics-apikey", "my-metrics-apikey",
                "--honeycomb-dataset", "my-dataset",
                "--honeycomb-traces-dataset", "my-traces-dataset",
                "--honeycomb-metrics-dataset", "my-metrics-dataset",
                "--honeycomb-samplerate", "5",
                "--honeycomb-endpoint", "my-endpoint",
                "--honeycomb-traces-endpoint", "my-traces-endpoint",
                "--honeycomb-metrics-endpoint", "my-metrics-endpoint",
                "--meter-names", "meter1,meter2",
                "--service-name", "my-service",
                "--service-version", "my-version",
                "--debug", "true"
            );

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
            Assert.True(options.Debug);
        }

        [Fact]
        public void CanParseOptionsFromInlineCommandLineArgs()
        {
            var options = HoneycombOptions.FromArgs(
                "--honeycomb-apikey=my-apikey",
                "--honeycomb-traces-apikey=my-traces-apikey",
                "--honeycomb-metrics-apikey=my-metrics-apikey",
                "--honeycomb-dataset=my-dataset",
                "--honeycomb-traces-dataset=my-traces-dataset",
                "--honeycomb-metrics-dataset=my-metrics-dataset",
                "--honeycomb-samplerate=5",
                "--honeycomb-endpoint=my-endpoint",
                "--honeycomb-traces-endpoint=my-traces-endpoint",
                "--honeycomb-metrics-endpoint=my-metrics-endpoint",
                "--meter-names=meter1,meter2",
                "--service-name=my-service",
                "--service-version=my-version",
                "--debug=true"
            );

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
            Assert.True(options.Debug);
        }

        [Fact]
>>>>>>> df7f3b7 (remove options unit tests related to fallbacks)
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