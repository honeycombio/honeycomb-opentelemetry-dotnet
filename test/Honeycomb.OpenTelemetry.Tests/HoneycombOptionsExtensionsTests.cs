using Xunit;

namespace Honeycomb.OpenTelemetry {
    public class HoneycombOptionsExtensionsTests
    {
        private const string ModernApiKey = "a142c03cf06936628e60f4";
        private const string ClassicApiKey = "a142c03cf06936628e60f4c6157fde46";

        [Theory]
        [InlineData("", "", "x-otlp-version=0.16.0,x-honeycomb-team=")]
        [InlineData(ModernApiKey, "", "x-otlp-version=0.16.0,x-honeycomb-team=apikey")]
        [InlineData(ModernApiKey, "dataset", "x-otlp-version=0.16.0,x-honeycomb-team=apikey")]
        [InlineData(ClassicApiKey, "", "x-otlp-version=0.16.0,x-honeycomb-team=apikey")]
        [InlineData(ClassicApiKey, "dataset", "x-otlp-version=0.16.0,x-honeycomb-team=apikey,x-honeycomb-dataset=dataset")]
        public void TracesHeaders(string apikey, string dataset, string expectedHeader) {
            var options = new HoneycombOptions
            {
                TracesApiKey = apikey,
                TracesDataset = dataset
            };
            Assert.Equal(expectedHeader, options.GetTraceHeaders());
        }

        [Theory]
        [InlineData(ModernApiKey, "", "x-otlp-version=0.16.0,x-honeycomb-team=apikey")]
        [InlineData(ClassicApiKey, "dataset", "x-otlp-version=0.16.0,x-honeycomb-team=apikey,x-honeycomb-dataset=dataset")]
        public void MetricsHeaders(string apikey, string dataset, string expectedHeader) {
            var options = new HoneycombOptions
            {
                MetricsApiKey = apikey,
                MetricsDataset = dataset
            };
            Assert.Equal(expectedHeader, options.GetMetricsHeaders());
        }
    }
}