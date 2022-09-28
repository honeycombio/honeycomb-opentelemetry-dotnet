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
        public void CanParseOptionsFromEnvironmentVariables()
        {
            // set up HoneycombOptions class
            var options = new HoneycombOptions { };
            // env var values
            var values = new Dictionary<string, string>
            {
                {"HONEYCOMB_API_KEY", "my-api-key-env-var"},
                {"HONEYCOMB_TRACES_API_KEY", "my-traces-api-key-env-var"},
                {"HONEYCOMB_METRICS_API_KEY", "my-metrics-api-key-env-var"},
                {"HONEYCOMB_DATASET", "my-dataset-env-var"},
                {"HONEYCOMB_TRACES_DATASET", "my-traces-dataset-env-var"},
                {"HONEYCOMB_METRICS_DATASET", "my-metrics-dataset-env-var"},
                {"HONEYCOMB_API_ENDPOINT", "my-endpoint-env-var"},
                {"HONEYCOMB_TRACES_ENDPOINT", "my-traces-endpoint-env-var"},
                {"HONEYCOMB_METRICS_ENDPOINT", "my-metrics-endpoint-env-var"},
                {"HONEYCOMB_SAMPLE_RATE", "20"},
                {"OTEL_SERVICE_NAME", "my-service-name-env-var"},
                {"SERVICE_VERSION", "my-service-version-env-var"},
                {"HONEYCOMB_ENABLE_LOCAL_VISUALIZATIONS", "false" },
                {"DEBUG", "false"}
            };

            // override HoneycombOptions with Environment options
            options.ApplyEnvironmentOptions(new EnvironmentOptions(values));

            Assert.Equal("my-api-key-env-var", options.ApiKey);
            Assert.Equal("my-traces-api-key-env-var", options.TracesApiKey);
            Assert.Equal("my-metrics-api-key-env-var", options.MetricsApiKey);
            Assert.Equal("my-dataset-env-var", options.Dataset);
            Assert.Equal("my-traces-dataset-env-var", options.TracesDataset);
            Assert.Equal("my-metrics-dataset-env-var", options.MetricsDataset);
            Assert.Equal("my-endpoint-env-var", options.Endpoint);
            Assert.Equal("my-traces-endpoint-env-var", options.TracesEndpoint);
            Assert.Equal("my-metrics-endpoint-env-var", options.MetricsEndpoint);
            Assert.Equal((uint)20, options.SampleRate);
            Assert.Equal("my-service-name-env-var", options.ServiceName);
            Assert.Equal("my-service-version-env-var", options.ServiceVersion);
            Assert.False(options.EnableLocalVisualizations);
            Assert.False(options.Debug);
        }

        [Fact]
        public void CanOverrideConfigWithEnvVars()
        {
            // read options from appsettings.json file
            var options = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build()
            .GetSection(HoneycombOptions.ConfigSectionName)
            .Get<HoneycombOptions>();

            // env var values
            var values = new Dictionary<string, string>
            {
                {"HONEYCOMB_API_KEY", "my-api-key-env-var"},
                {"HONEYCOMB_TRACES_API_KEY", "my-traces-api-key-env-var"},
                {"HONEYCOMB_METRICS_API_KEY", "my-metrics-api-key-env-var"},
                {"HONEYCOMB_DATASET", "my-dataset-env-var"},
                {"HONEYCOMB_TRACES_DATASET", "my-traces-dataset-env-var"},
                {"HONEYCOMB_METRICS_DATASET", "my-metrics-dataset-env-var"},
                {"HONEYCOMB_API_ENDPOINT", "my-endpoint-env-var"},
                {"HONEYCOMB_TRACES_ENDPOINT", "my-traces-endpoint-env-var"},
                {"HONEYCOMB_METRICS_ENDPOINT", "my-metrics-endpoint-env-var"},
                {"HONEYCOMB_SAMPLE_RATE", "20"},
                {"OTEL_SERVICE_NAME", "my-service-name-env-var"},
                {"SERVICE_VERSION", "my-service-version-env-var"},
                {"HONEYCOMB_ENABLE_LOCAL_VISUALIZATIONS", "false" },
                {"DEBUG", "false"}
            };

            options.ApplyEnvironmentOptions(new EnvironmentOptions(values));

            Assert.Equal("my-api-key-env-var", options.ApiKey);
            Assert.Equal("my-traces-api-key-env-var", options.TracesApiKey);
            Assert.Equal("my-metrics-api-key-env-var", options.MetricsApiKey);
            Assert.Equal("my-dataset-env-var", options.Dataset);
            Assert.Equal("my-traces-dataset-env-var", options.TracesDataset);
            Assert.Equal("my-metrics-dataset-env-var", options.MetricsDataset);
            Assert.Equal("my-endpoint-env-var", options.Endpoint);
            Assert.Equal("my-traces-endpoint-env-var", options.TracesEndpoint);
            Assert.Equal("my-metrics-endpoint-env-var", options.MetricsEndpoint);
            Assert.Equal((uint)20, options.SampleRate);
            Assert.Equal("my-service-name-env-var", options.ServiceName);
            Assert.Equal("my-service-version-env-var", options.ServiceVersion);
            Assert.False(options.EnableLocalVisualizations);
            Assert.False(options.Debug);
        }

        [Fact]
        public void CanFallbackToDefault_Config()
        {
            // set up empty HoneycombOptions class
            var options = new HoneycombOptions { };
            Assert.Null(options.ApiKey);
            Assert.Null(options.TracesApiKey);
            Assert.Null(options.MetricsApiKey);
            Assert.Null(options.Dataset);
            Assert.Null(options.TracesDataset);
            Assert.Null(options.MetricsDataset);
            Assert.Equal(HoneycombOptions.DefaultEndpoint, options.Endpoint);
            Assert.Null(options.TracesEndpoint);
            Assert.Null(options.MetricsEndpoint);
            Assert.Equal(HoneycombOptions.DefaultSampleRate, options.SampleRate);
            Assert.Equal(HoneycombOptions.SDefaultServiceName, options.ServiceName);
            Assert.Equal(HoneycombOptions.SDefaultServiceVersion, options.ServiceVersion);
            Assert.False(options.EnableLocalVisualizations);
            Assert.False(options.Debug);
        }

        [Fact]
        public void UsesGenericValuesIfTracesSpecificValuesAreNotSet_Config()
        {
            var options = new HoneycombOptions
            {
                Endpoint = "http://collector:4318",
                ApiKey = "my-api-key",
                Dataset = "my-dataset"
            };

            Assert.Equal("http://collector:4318/", options.GetTracesEndpoint());
            Assert.Equal("my-api-key", options.GetTracesApiKey());
            Assert.Equal("my-dataset", options.GetTracesDataset());
        }

        [Fact]
        public void UsesGenericValuesIfMetricsSpecificValuesAreNotSet_Config()
        {
            var options = new HoneycombOptions
            {
                Endpoint = "http://collector:4318",
                ApiKey = "my-api-key",
                Dataset = "my-dataset"
            };

            Assert.Equal("http://collector:4318/", options.GetMetricsEndpoint());
            Assert.Equal("my-api-key", options.GetMetricsApiKey());
            // Should not fall override metrics dataset
            Assert.NotEqual("my-dataset", options.MetricsDataset);
        }

        [Fact]
        public void UsesTracesSpecificValuesIfSet_Config()
        {
            var options = new HoneycombOptions
            {
                TracesEndpoint = "http://collector:4318",
                TracesApiKey = "my-api-key",
                TracesDataset = "my-dataset"
            };
            Assert.Equal("http://collector:4318", options.GetTracesEndpoint());
            Assert.Equal("my-api-key", options.GetTracesApiKey());
            Assert.Equal("my-dataset", options.GetTracesDataset());
        }

        [Fact]
        public void UsesMetricsSpecificValuesIfSet_Config()
        {
            var options = new HoneycombOptions
            {
                MetricsEndpoint = "http://collector:4318",
                MetricsApiKey = "my-api-key",
            };
            Assert.Equal("http://collector:4318/", options.GetMetricsEndpoint());
            Assert.Equal("my-api-key", options.GetMetricsApiKey());
        }

        [Fact]
        public void UseTracesSpecificValuesOverGenericValues_Config()
        {
            var options = new HoneycombOptions
            {
                Endpoint = "http://collector:4318",
                ApiKey = "my-api-key",
                Dataset = "my-dataset",
                TracesEndpoint = "http://collector:4318/v1/traces",
                TracesApiKey = "my-api-key-traces",
                TracesDataset = "my-dataset-traces"
            };

            Assert.Equal("http://collector:4318/v1/traces", options.GetTracesEndpoint());
            Assert.Equal("my-api-key-traces", options.GetTracesApiKey());
            Assert.Equal("my-dataset-traces", options.GetTracesDataset());
        }

        [Fact]
        public void UseMetricsSpecificValuesOverGenericValues_Config()
        {
            var options = new HoneycombOptions
            {
                Endpoint = "http://collector:4318",
                ApiKey = "my-api-key",
                MetricsEndpoint = "http://collector:4318/v1/metrics",
                MetricsApiKey = "my-api-key-metrics",
            };

            Assert.Equal("http://collector:4318/v1/metrics", options.GetMetricsEndpoint());
            Assert.Equal("my-api-key-metrics", options.GetMetricsApiKey());
        }

        [Fact]
        public void UsesGenericValuesIfTracesSpecificValuesAreNotSet_EnvVars()
        {
            var options = new HoneycombOptions { };
            var values = new Dictionary<string, string>
            {
                {"HONEYCOMB_API_KEY", "my-api-key-env-var"},
                {"HONEYCOMB_DATASET", "my-dataset-env-var"},
                {"HONEYCOMB_API_ENDPOINT", "http://collector:4318/"},
            };
            options.ApplyEnvironmentOptions(new EnvironmentOptions(values));

            Assert.Equal("http://collector:4318/", options.GetTracesEndpoint());
            Assert.Equal("my-api-key-env-var", options.GetTracesApiKey());
            Assert.Equal("my-dataset-env-var", options.GetTracesDataset());
        }

        [Fact]
        public void UsesGenericValuesIfMetricsSpecificValuesAreNotSet_EnvVars()
        {
            var options = new HoneycombOptions { };
            var values = new Dictionary<string, string>
            {
                {"HONEYCOMB_API_KEY", "my-api-key-env-var"},
                {"HONEYCOMB_DATASET", "my-dataset-env-var"},
                {"HONEYCOMB_API_ENDPOINT", "http://collector:4318/"},
            };
            options.ApplyEnvironmentOptions(new EnvironmentOptions(values));

            Assert.Equal("http://collector:4318/", options.GetMetricsEndpoint());
            Assert.Equal("my-api-key-env-var", options.GetMetricsApiKey());
        }

        [Fact]
        public void UsesTracesSpecificValuesIfSet_EnvVars()
        {
            var options = new HoneycombOptions { };
            var values = new Dictionary<string, string>
            {
                {"HONEYCOMB_TRACES_API_KEY", "my-traces-api-key-env-var"},
                {"HONEYCOMB_TRACES_DATASET", "my-traces-dataset-env-var"},
                {"HONEYCOMB_TRACES_ENDPOINT", "http://collector:4318/v1/traces"},
            };
            options.ApplyEnvironmentOptions(new EnvironmentOptions(values));

            Assert.Equal("http://collector:4318/v1/traces", options.GetTracesEndpoint());
            Assert.Equal("my-traces-api-key-env-var", options.GetTracesApiKey());
            Assert.Equal("my-traces-dataset-env-var", options.GetTracesDataset());
        }

        [Fact]
        public void UsesMetricsSpecificValuesIfSet_EnvVars()
        {
            var options = new HoneycombOptions { };
            var values = new Dictionary<string, string>
            {
                {"HONEYCOMB_METRICS_API_KEY", "my-metrics-api-key-env-var"},
                {"HONEYCOMB_METRICS_ENDPOINT", "http://collector:4318/v1/metrics"},
            };
            options.ApplyEnvironmentOptions(new EnvironmentOptions(values));

            Assert.Equal("http://collector:4318/v1/metrics", options.GetMetricsEndpoint());
            Assert.Equal("my-metrics-api-key-env-var", options.GetMetricsApiKey());
        }

        [Fact]
        public void UseTracesSpecificValuesOverGenericValues_EnvVars()
        {
            var options = new HoneycombOptions { };
            var values = new Dictionary<string, string>
            {
                {"HONEYCOMB_API_KEY", "my-api-key-env-var"},
                {"HONEYCOMB_DATASET", "my-dataset-env-var"},
                {"HONEYCOMB_API_ENDPOINT", "http://collector:4318/"},
                {"HONEYCOMB_TRACES_API_KEY", "my-traces-api-key-env-var"},
                {"HONEYCOMB_TRACES_DATASET", "my-traces-dataset-env-var"},
                {"HONEYCOMB_TRACES_ENDPOINT", "http://collector:4318/v1/traces"},
            };
            options.ApplyEnvironmentOptions(new EnvironmentOptions(values));

            Assert.Equal("http://collector:4318/v1/traces", options.GetTracesEndpoint());
            Assert.Equal("my-traces-api-key-env-var", options.GetTracesApiKey());
            Assert.Equal("my-traces-dataset-env-var", options.GetTracesDataset());
        }

        [Fact]
        public void UseMetricsSpecificValuesOverGenericValues_EnvVars()
        {
            var options = new HoneycombOptions { };
            var values = new Dictionary<string, string>
            {
                {"HONEYCOMB_API_KEY", "my-api-key-env-var"},
                {"HONEYCOMB_API_ENDPOINT", "http://collector:4318/"},
                {"HONEYCOMB_METRICS_API_KEY", "my-metrics-api-key-env-var"},
                {"HONEYCOMB_METRICS_ENDPOINT", "http://collector:4318/v1/metrics"},
            };
            options.ApplyEnvironmentOptions(new EnvironmentOptions(values));

            Assert.Equal("http://collector:4318/v1/metrics", options.GetMetricsEndpoint());
            Assert.Equal("my-metrics-api-key-env-var", options.GetMetricsApiKey());
        }

        [Fact]
        public void AppendsTracesPathIfProtocolIsHttpProtobuf_Config()
        {
            var options = new HoneycombOptions
            {
                Endpoint = "http://collector:4318/"
            };
            var values = new Dictionary<string, string>
            {
                {"OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf"},
            };

            options.ApplyEnvironmentOptions(new EnvironmentOptions(values));
            Assert.Equal("http://collector:4318/v1/traces", options.GetTracesEndpoint());
        }

        [Fact]
        public void AppendsTracesPathIfProtocolIsHttpJson_Config()
        {
            var options = new HoneycombOptions
            {
                Endpoint = "http://collector:4318/"
            };
            var values = new Dictionary<string, string>
            {
                {"OTEL_EXPORTER_OTLP_PROTOCOL", "http/json"},
            };

            options.ApplyEnvironmentOptions(new EnvironmentOptions(values));
            Assert.Equal("http://collector:4318/v1/traces", options.GetTracesEndpoint());
        }

        [Fact]
        public void DoesNotAppendTracesPathIfProtocolIsGrpc_Config()
        {
            var options = new HoneycombOptions
            {
                Endpoint = "http://collector:4317/"
            };
            var values = new Dictionary<string, string>
            {
                {"OTEL_EXPORTER_OTLP_PROTOCOL", "grpc"},
            };

            options.ApplyEnvironmentOptions(new EnvironmentOptions(values));
            Assert.Equal("http://collector:4317/", options.GetTracesEndpoint());
            Assert.DoesNotContain("/v1/traces", options.GetTracesEndpoint());
        }

        [Fact]
        public void AppendsTracesPathIfProtocolIsHttpProtobuf_EnvVars()
        {
            var options = new HoneycombOptions { };
            var values = new Dictionary<string, string>
            {
                {"OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf"},
                {"HONEYCOMB_API_ENDPOINT", "http://collector:4318/"}
            };

            options.ApplyEnvironmentOptions(new EnvironmentOptions(values));
            Assert.Equal("http://collector:4318/v1/traces", options.GetTracesEndpoint());
        }

        [Fact]
        public void AppendsTracesPathIfProtocolIsHttpJson_EnvVars()
        {
            var options = new HoneycombOptions { };
            var values = new Dictionary<string, string>
            {
                {"OTEL_EXPORTER_OTLP_PROTOCOL", "http/json"},
                {"HONEYCOMB_API_ENDPOINT", "http://collector:4318/"}
            };

            options.ApplyEnvironmentOptions(new EnvironmentOptions(values));
            Assert.Equal("http://collector:4318/v1/traces", options.GetTracesEndpoint());
        }
        [Fact]
        public void DoesNotAppendTracesPathIfProtocolIsGrpc_EnvVars()
        {
            var options = new HoneycombOptions { };
            var values = new Dictionary<string, string>
            {
                {"OTEL_EXPORTER_OTLP_PROTOCOL", "grpc"},
                {"HONEYCOMB_API_ENDPOINT", "http://collector:4317/"}
            };

            options.ApplyEnvironmentOptions(new EnvironmentOptions(values));
            Assert.Equal("http://collector:4317/", options.GetTracesEndpoint());
            Assert.DoesNotContain("/v1/traces", options.GetTracesEndpoint());
        }

        [Fact]
        public void DoesNotAppendTracesPathToTracesEndpoint_Config()
        {
            var options = new HoneycombOptions
            {
                TracesEndpoint = "http://collector:4318/"
            };
            var values = new Dictionary<string, string>
            {
                {"OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf"},
            };

            options.ApplyEnvironmentOptions(new EnvironmentOptions(values));
            Assert.Equal("http://collector:4318/", options.GetTracesEndpoint());
        }

        [Fact]
        public void DoesNotAppendTracesPathToTracesEndpoint_EnvVars()
        {
            var options = new HoneycombOptions { };
            var values = new Dictionary<string, string>
            {
                {"OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf"},
                {"HONEYCOMB_TRACES_ENDPOINT", "http://collector:4318/"}
            };

            options.ApplyEnvironmentOptions(new EnvironmentOptions(values));
            Assert.Equal("http://collector:4318/", options.GetTracesEndpoint());

        }

        [Fact]
        public void Legacy_key_length()
        {
            var options = new HoneycombOptions { ApiKey = "1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a" };
            Assert.True(HoneycombOptions.IsClassicKey(options.ApiKey));
        }
        [Fact]
        public void Not_legacy_key_length()
        {
            var options = new HoneycombOptions { ApiKey = "specialenvkey" };
            Assert.False(HoneycombOptions.IsClassicKey(options.ApiKey));
        }
    }
}