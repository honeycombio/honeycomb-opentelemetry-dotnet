using Microsoft.Extensions.Configuration;
using Xunit;

namespace Honeycomb.OpenTelemetry.Tests
{
    public class HoneycombOptionsHelperTests
    {
        [Fact]
        public void CanParseOptionsFromSpacedCommandLineArgs()
        {
            var options = HoneycombOptions.FromArgs(
                "--honeycomb-apikey", "my-apikey",
                "--honeycomb-dataset", "my-dataset",
                "--honeycomb-samplerate", "5",
                "--honeycomb-endpoint", "my-endpoint",
                "--service-name", "my-service",
                "--service-version", "my-version",
                "--instrument-http", "false",
                "--instrument-sql", "false",
                "--instrument-grpc", "false",
                "--instrument-redis", "false"
            );

            Assert.Equal("my-apikey", options.ApiKey);
            Assert.Equal("my-dataset", options.Dataset);
            Assert.Equal((uint)5, options.SampleRate);
            Assert.Equal("my-endpoint", options.Endpoint);
            Assert.Equal("my-service", options.ServiceName);
            Assert.Equal("my-version", options.ServiceVersion);
            Assert.False(options.InstrumentHttpClient);
            Assert.False(options.InstrumentSqlClient);
            Assert.False(options.InstrumentGrpcClient);
            Assert.False(options.InstrumentStackExchangeRedisClient);
        }

        [Fact]
        public void CanParseOptionsFromInlineCommandLineArgs()
        {
            var options = HoneycombOptions.FromArgs(
                "--honeycomb-apikey=my-apikey",
                "--honeycomb-dataset=my-dataset",
                "--honeycomb-samplerate=5",
                "--honeycomb-endpoint=my-endpoint",
                "--service-name=my-service",
                "--service-version=my-version",
                "--instrument-http=false",
                "--instrument-sql=false",
                "--instrument-grpc=false",
                "--instrument-redis=false");

            Assert.Equal("my-apikey", options.ApiKey);
            Assert.Equal("my-dataset", options.Dataset);
            Assert.Equal((uint)5, options.SampleRate);
            Assert.Equal("my-endpoint", options.Endpoint);
            Assert.Equal("my-service", options.ServiceName);
            Assert.Equal("my-version", options.ServiceVersion);
            Assert.False(options.InstrumentHttpClient);
            Assert.False(options.InstrumentSqlClient);
            Assert.False(options.InstrumentGrpcClient);
            Assert.False(options.InstrumentStackExchangeRedisClient);
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
            Assert.False(options.InstrumentHttpClient);
            Assert.False(options.InstrumentSqlClient);
            Assert.False(options.InstrumentGrpcClient);
            Assert.False(options.InstrumentStackExchangeRedisClient);
        }
    }
}