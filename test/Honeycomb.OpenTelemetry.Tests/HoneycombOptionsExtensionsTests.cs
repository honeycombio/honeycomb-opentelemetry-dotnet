using Xunit;

namespace Honeycomb.OpenTelemetry {
    public class HoneycombOptionsExtensionsTests
    {
        private const string ModernApiKey = "6936628e60f4c6157fde46";
        private const string ClassicApiKey = "a142c03cf06936628e60f4c6157fde46";

        [Theory]
        [InlineData("", "", "x-otlp-version=0.18.0,x-honeycomb-team=")]
        [InlineData(ModernApiKey, "", "x-otlp-version=0.18.0,x-honeycomb-team=6936628e60f4c6157fde46")]
        [InlineData(ModernApiKey, "dataset", "x-otlp-version=0.18.0,x-honeycomb-team=6936628e60f4c6157fde46")]
        [InlineData(ClassicApiKey, "", "x-otlp-version=0.18.0,x-honeycomb-team=a142c03cf06936628e60f4c6157fde46")]
        [InlineData(ClassicApiKey, "dataset", "x-otlp-version=0.18.0,x-honeycomb-team=a142c03cf06936628e60f4c6157fde46,x-honeycomb-dataset=dataset")]
        public void TracesHeaders(string apikey, string dataset, string expectedHeader) {
            var options = new HoneycombOptions
            {
                TracesApiKey = apikey,
                TracesDataset = dataset
            };
            Assert.Equal(expectedHeader, options.GetTraceHeaders());
        }

        [Theory]
        [InlineData("", "", "x-otlp-version=0.18.0,x-honeycomb-team=,x-honeycomb-dataset=")]
        [InlineData(ModernApiKey, "", "x-otlp-version=0.18.0,x-honeycomb-team=6936628e60f4c6157fde46,x-honeycomb-dataset=")]
        [InlineData(ModernApiKey, "dataset", "x-otlp-version=0.18.0,x-honeycomb-team=6936628e60f4c6157fde46,x-honeycomb-dataset=dataset")]
        [InlineData(ClassicApiKey, "", "x-otlp-version=0.18.0,x-honeycomb-team=a142c03cf06936628e60f4c6157fde46,x-honeycomb-dataset=")]
        [InlineData(ClassicApiKey, "dataset", "x-otlp-version=0.18.0,x-honeycomb-team=a142c03cf06936628e60f4c6157fde46,x-honeycomb-dataset=dataset")]
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