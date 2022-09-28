using Microsoft.Extensions.Configuration;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace Honeycomb.OpenTelemetry
{
    /// <summary>
    /// Configuration options used to configure <see cref="TracerProviderBuilder"/> to send telemetry data to Honeycomb.
    /// </summary>
    public class HoneycombOptions
    {
        private const string OtlpVersion = "0.16.0";
        private const string OtelExporterOtlpProtcolHttp = "http/protobuf";
        private const string OtelExporterOtlpProtcolGrpc = "grpc";
        private const string OtelExporterHttpTracesPath = "/v1/traces";

        private bool isHttp = false;

        /// <summary>
        /// Default service name if service name is not provided.
        /// </summary>
        internal static readonly string SDefaultServiceName = $"unknown_service:{System.Diagnostics.Process.GetCurrentProcess().ProcessName}";
        internal static readonly string SDefaultServiceVersion = "unknown_service_version";

        /// <summary>
        /// Name of the Honeycomb section of IConfiguration
        /// </summary>
        public const string ConfigSectionName = "Honeycomb";

        /// <summary>
        /// Default API endpoint.
        /// </summary>
        public const string DefaultEndpoint = "https://api.honeycomb.io:443";

        /// <summary>
        /// Default sample rate - sample everything.
        /// </summary>
        public const uint DefaultSampleRate = 1;

        /// <summary>
        /// API key used to send telemetry data to Honeycomb.
        /// <para/>
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Returns whether the provided API key is a legacy key.
        /// </summary>
        /// <remarks>
        /// Legacy keys have 32 characters.
        /// </remarks>
        internal static bool IsClassicKey(string apikey) => apikey?.Length == 32;

        /// <summary>
        /// Write links to honeycomb traces as they come in
        /// </summary>
        public bool EnableLocalVisualizations { get; set; } = false;

        /// <summary>
        /// API key used to send trace telemetry data to Honeycomb. Defaults to <see cref="ApiKey"/>.
        /// </summary>
        public string TracesApiKey { get; set; }

        /// <summary>
        /// API key used to send metrics telemetry data to Honeycomb. Defaults to <see cref="ApiKey"/>.
        /// </summary>
        public string MetricsApiKey { get; set; }

        /// <summary>
        /// Honeycomb dataset to store telemetry data.
        /// <para/>
        /// </summary>
        public string Dataset { get; set; }

        /// <summary>
        /// Honeycomb dataset to store trace telemetry data. Defaults to <see cref="Dataset"/>.
        /// </summary>
        public string TracesDataset { get; set; }

        /// <summary>
        /// Honeycomb dataset to store metrics telemetry data. Defaults to "null".
        /// <para/>
        /// Required to enable metrics.
        /// </summary>
        public string MetricsDataset { get; set; }

        /// <summary>
        /// API endpoint to send telemetry data. Defaults to <see cref="DefaultEndpoint"/>.
        /// </summary>
        public string Endpoint { get; set; } = DefaultEndpoint;

        /// <summary>
        /// API endpoint to send telemetry data. Defaults to <see cref="Endpoint"/>.
        /// </summary>
        public string TracesEndpoint { get; set; }

        /// <summary>
        /// API endpoint to send telemetry data. Defaults to <see cref="Endpoint"/>.
        /// </summary>
        public string MetricsEndpoint { get; set; }

        /// <summary>
        /// Sample rate for sending telemetry data. Defaults to <see cref="DefaultSampleRate"/>.
        /// <para/>
        /// See <see cref="DeterministicSampler"/> for more details on how sampling is applied.
        /// </summary>
        public uint SampleRate { get; set; } = DefaultSampleRate;

        /// <summary>
        /// Service name used to identify application. Defaults to unknown_process:processname.
        /// </summary>
        public string ServiceName { get; set; } = SDefaultServiceName;

        /// <summary>
        /// Service version. Defaults to application assembly information version.
        /// </summary>
        public string ServiceVersion { get; set; } = SDefaultServiceVersion;

        /// <summary>
        /// (Optional) Additional <see cref="Meter"/> names for generating metrics.
        /// <see cref="ServiceName"/> is configured as a meter name by default.
        /// </summary>
        public List<string> MeterNames { get; set; } = new List<string>();

        /// <summary>
        /// The <see cref="ResourceBuilder" /> to use to add Resource attributes to.
        /// A custom ResouceBuilder can be used to set additional resources and then passed here to add
        /// Honeycomb attributes.
        /// </summary>
        public ResourceBuilder ResourceBuilder { get; set; } = ResourceBuilder.CreateDefault();

        /// <summary>
        /// Determines whether the <see cref="BaggageSpanProcessor"/> is added when configuring a <see cref="TracerProviderBuilder"/>.
        /// </summary>
        public bool AddBaggageSpanProcessor { get; set; } = true;

        /// <summary>
        /// Determines whether the <see cref="DeterministicSampler"/> sampler is added when configuring a <see cref="TracerProviderBuilder"/>.
        /// </summary>
        public bool AddDeterministicSampler { get; set; } = true;

        /// <summary>
        /// If set to true, enables the console span exporter for local debugging.
        /// </summary>
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Applies environment variable option overrides.
        /// </summary>
        internal void ApplyEnvironmentOptions(EnvironmentOptions environmentOptions)
        {
            if (!string.IsNullOrWhiteSpace(environmentOptions.ApiKey))
            {
                ApiKey = environmentOptions.ApiKey;
            }

            if (!string.IsNullOrWhiteSpace(environmentOptions.TracesApiKey))
            {
                TracesApiKey = environmentOptions.TracesApiKey;
            }

            if (!string.IsNullOrWhiteSpace(environmentOptions.MetricsApiKey))
            {
                MetricsApiKey = environmentOptions.MetricsApiKey;
            }

            if (!string.IsNullOrWhiteSpace(environmentOptions.ApiEndpoint))
            {
                Endpoint = environmentOptions.ApiEndpoint;
            }

            if (!string.IsNullOrWhiteSpace(environmentOptions.Dataset))
            {
                Dataset = environmentOptions.Dataset;
            }

            if (!string.IsNullOrWhiteSpace(environmentOptions.TracesDataset))
            {
                TracesDataset = environmentOptions.TracesDataset;
            }

            if (!string.IsNullOrWhiteSpace(environmentOptions.MetricsDataset))
            {
                MetricsDataset = environmentOptions.MetricsDataset;
            }

            if (!string.IsNullOrWhiteSpace(environmentOptions.TracesEndpoint))
            {
                TracesEndpoint = environmentOptions.TracesEndpoint;
            }

            if (!string.IsNullOrWhiteSpace(environmentOptions.MetricsEndpoint))
            {
                MetricsEndpoint = environmentOptions.MetricsEndpoint;
            }

            if (!string.IsNullOrWhiteSpace(environmentOptions.ServiceName))
            {
                ServiceName = environmentOptions.ServiceName;
            }

            if (!string.IsNullOrWhiteSpace(environmentOptions.ServiceVersion))
            {
                ServiceVersion = environmentOptions.ServiceVersion;
            }

            if (!string.IsNullOrWhiteSpace(environmentOptions.EnableLocalVisualizationsValue))
            {
                EnableLocalVisualizations = environmentOptions.EnableLocalVisualizations;
            }

            if (!string.IsNullOrWhiteSpace(environmentOptions.DebugValue))
            {
                Debug = environmentOptions.Debug;
            }

            if (!string.IsNullOrWhiteSpace(environmentOptions.SampleRateValue))
            {
                SampleRate = environmentOptions.SampleRate;
            }

            if (!string.IsNullOrWhiteSpace(environmentOptions.OtelExporterOtlpProtocol))
            {
                isHttp = true;
            }
        }

        /// <summary>
        /// Computes the final traces endpoint.
        /// </summary>
        internal string GetTracesEndpoint()
        {
            var endpoint = new UriBuilder(Endpoint);
            if (isHttp)
            {
                endpoint.Path = OtelExporterHttpTracesPath;
            }
            return TracesEndpoint ?? endpoint.ToString();
        }

        internal string GetTracesApiKey()
        {
            return TracesApiKey ?? ApiKey;
        }

        internal string GetTracesDataset()
        {
            return TracesDataset ?? Dataset;
        }

        /// <summary>
        /// Gets the <see cref="MetricsEndpoint" /> or falls back to the generic <see cref="Endpoint" />.
        /// </summary>
        internal string GetMetricsEndpoint()
        {
            return new UriBuilder(MetricsEndpoint ?? Endpoint).ToString();
        }

        /// <summary>
        /// Gets the <see cref="MetricsApiKey" /> or falls back to the generic <see cref="ApiKey" />.
        /// </summary>
        internal string GetMetricsApiKey()
        {
            return MetricsApiKey ?? ApiKey;
        }

        internal string GetTraceHeaders() => GetTraceHeaders(TracesApiKey, TracesDataset);

        internal static string GetTraceHeaders(string apikey, string dataset)
        {
            var headers = new List<string>
            {
                $"x-otlp-version={OtlpVersion}",
                $"x-honeycomb-team={apikey}"
            };

            if (IsClassicKey(apikey))
            {
                // if the key is legacy, add dataset to the header
                if (!string.IsNullOrWhiteSpace(dataset))
                {
                    headers.Add($"x-honeycomb-dataset={dataset}");
                }
                else
                {
                    // if legacy key and missing dataset, warn on missing dataset
                    Console.WriteLine($"WARN: {EnvironmentOptions.GetErrorMessage("dataset", "HONEYCOMB_DATASET")}.");
                }
            }

            return string.Join(",", headers);
        }

        internal string GetMetricsHeaders() => GetMetricsHeaders(MetricsApiKey, MetricsDataset);

        internal static string GetMetricsHeaders(string apikey, string dataset)
        {
            var headers = new List<string>
            {
                $"x-otlp-version={OtlpVersion}",
                $"x-honeycomb-team={apikey}",
                $"x-honeycomb-dataset={dataset}"
            };

            return string.Join(",", headers);
        }
    }
}