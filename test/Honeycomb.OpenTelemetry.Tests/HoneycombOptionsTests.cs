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
                    .GetSection(HoneycombOptions.Honeycomb)
                    .Get<HoneycombOptions>()
            ;

            Assert.Equal("my-apikey", options.ApiKey);
            Assert.Equal("my-dataset", options.Dataset);
            Assert.Equal((uint)5, options.SampleRate);
            Assert.Equal("my-endpoint", options.Endpoint);
            Assert.Equal("my-service", options.ServiceName);
            Assert.Equal("my-version", options.ServiceVersion);
        }
    }
}