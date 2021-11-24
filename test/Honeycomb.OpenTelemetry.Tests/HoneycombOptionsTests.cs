using Microsoft.Extensions.Configuration;
using Xunit;

namespace Honeycomb.OpenTelemetry
{
    public class HoneycombOptionsHelperTests
    {
        [Fact]
        public void CanParseOptionsFromSpacedCommandLineArgs()
        {
            var options = HoneycombOptions.FromArgs(
                new string[]
                {
                    "--honeycomb-apikey", "my-apikey",
                    "--honeycomb-dataset", "my-dataset",
                    "--honeycomb-samplerate", "5",
                    "--honeycomb-endpoint", "my-endpoint",
                    "--service-name", "my-service",
                    "--service-version", "my-version"
                }
            );

            Assert.Equal("my-apikey", options.ApiKey);
            Assert.Equal("my-dataset", options.Dataset);
            Assert.Equal((uint)5, options.SampleRate);
            Assert.Equal("my-endpoint", options.Endpoint);
            Assert.Equal("my-service", options.ServiceName);
            Assert.Equal("my-version", options.ServiceVersion);
        }

        [Fact]
        public void CanParseOptionsFromInlineCommandLineArgs()
        {
            var options = HoneycombOptions.FromArgs(
                new string[]
                {
                    "--honeycomb-apikey=my-apikey",
                    "--honeycomb-dataset=my-dataset",
                    "--honeycomb-samplerate=5",
                    "--honeycomb-endpoint=my-endpoint",
                    "--service-name=my-service",
                    "--service-version=my-version"
                }
            );

            Assert.Equal("my-apikey", options.ApiKey);
            Assert.Equal("my-dataset", options.Dataset);
            Assert.Equal((uint)5, options.SampleRate);
            Assert.Equal("my-endpoint", options.Endpoint);
            Assert.Equal("my-service", options.ServiceName);
            Assert.Equal("my-version", options.ServiceVersion);
        }

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
            Assert.Equal("my-dataset", options.Dataset);
            Assert.Equal((uint)5, options.SampleRate);
            Assert.Equal("my-endpoint", options.Endpoint);
            Assert.Equal("my-service", options.ServiceName);
            Assert.Equal("my-version", options.ServiceVersion);
        }

        [Fact]
        public void ApiKeyFallbacks()
        {
            var options = new HoneycombOptions
            {
                ApiKey = "mykey",
            };

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
            var options = new HoneycombOptions
            {
                Dataset = "dataset",
            };

            Assert.Equal("dataset", options.Dataset);
            Assert.Equal("dataset", options.TracesDataset);
            Assert.Equal(null, options.MetricsDataset); // do not fall back metrics dataset

            options.TracesDataset = "traces";
            options.MetricsDataset = "metrics";

            Assert.Equal("dataset", options.Dataset);
            Assert.Equal("traces", options.TracesDataset);
            Assert.Equal("metrics", options.MetricsDataset);
        }

        [Fact]
        public void EndpointFallbacks()
        {
            var options = new HoneycombOptions
            {
                Endpoint = "endpoint",
            };

            Assert.Equal("endpoint", options.Endpoint);
            Assert.Equal("endpoint", options.TracesEndpoint);
            Assert.Equal("endpoint", options.MetricsEndpoint);

            options.TracesEndpoint = "traces";
            options.MetricsEndpoint = "metrics";

            Assert.Equal("endpoint", options.Endpoint);
            Assert.Equal("traces", options.TracesEndpoint);
            Assert.Equal("metrics", options.MetricsEndpoint);
        }
    }
}