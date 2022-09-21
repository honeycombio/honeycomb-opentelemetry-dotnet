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

        /// <summary>
        /// Default service name if service name is not provided.
        /// </summary>
        internal static readonly string SDefaultServiceName = GetDefaultServiceName();
        private static readonly string SDefaultServiceVersion = "{unknown_service_version}";

        private static string GetDefaultServiceName()
        {
            var serviceName = "unknown_service";
            try
            {
                serviceName += $":{System.Diagnostics.Process.GetCurrentProcess().ProcessName}";
            }
            catch (PlatformNotSupportedException)
            {
                // some platforms do not have access, ignore
            }
            catch (Exception exception)
            {
                Console.WriteLine($"ERROR: unable to determine process name - {exception.ToString()}");
            }

            return serviceName;
        }

        private string _tracesApiKey;
        private string _metricsApiKey;
        private string _tracesDataset;
        private string _tracesEndpoint;
        private string _metricsEndpoint;

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
        /// Returns whether API key used to send trace telemetry is a legacy key.
        /// </summary>
        /// <remarks>
        /// Legacy keys have 32 characters.
        /// </remarks>
        internal bool IsTracesLegacyKey() => IsClassicKey(TracesApiKey);

        /// <summary>
        /// Returns whether API key used to send metrics telemetry is a legacy key.
        /// </summary>
        /// <remarks>
        /// Legacy keys have 32 characters.
        /// </remarks>
        internal bool IsMetricsLegacyKey() => IsClassicKey(MetricsApiKey);

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
        public string TracesApiKey
        {
            get { return _tracesApiKey ?? ApiKey; }
            set { _tracesApiKey = value; }
        }

        /// <summary>
        /// API key used to send metrics telemetry data to Honeycomb. Defaults to <see cref="ApiKey"/>.
        /// </summary>
        public string MetricsApiKey
        {
            get { return _metricsApiKey ?? ApiKey; }
            set { _metricsApiKey = value; }
        }

        /// <summary>
        /// Honeycomb dataset to store telemetry data.
        /// <para/>
        /// </summary>
        public string Dataset { get; set; }

        /// <summary>
        /// Honeycomb dataset to store trace telemetry data. Defaults to <see cref="Dataset"/>.
        /// </summary>
        public string TracesDataset
        {
            get { return _tracesDataset ?? Dataset; }
            set { _tracesDataset = value; }
        }

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
        public string TracesEndpoint
        {
            get { return _tracesEndpoint ?? Endpoint; }
            set { _tracesEndpoint = value; }
        }

        /// <summary>
        /// API endpoint to send telemetry data. Defaults to <see cref="Endpoint"/>.
        /// </summary>
        public string MetricsEndpoint
        {
            get { return _metricsEndpoint ?? Endpoint; }
            set { _metricsEndpoint = value; }
        }

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

        private static readonly Dictionary<string, string> CommandLineSwitchMap = new Dictionary<string, string>
        {
            { "--honeycomb-apikey", "apikey" },
            { "--honeycomb-traces-apikey", "tracesapikey" },
            { "--honeycomb-metrics-apikey", "metricsapikey" },
            { "--honeycomb-dataset", "dataset" },
            { "--honeycomb-traces-dataset", "tracesdataset" },
            { "--honeycomb-metrics-dataset", "metricsdataset" },
            { "--honeycomb-endpoint", "endpoint" },
            { "--honeycomb-traces-endpoint", "tracesendpoint" },
            { "--honeycomb-metrics-endpoint", "metricsendpoint" },
            { "--honeycomb-samplerate", "samplerate" },
            { "--honeycomb-enable-local-visualizations", "enablelocalvisualizations" },
            { "--honeycomb-add-baggage-span-processor", "addBaggageSpanProcessor" },
            { "--honeycomb-add-determinisitc-sampler", "addDeterministicSampler" },
            { "--service-name", "servicename" },
            { "--service-version", "serviceversion" },
            { "--meter-names", "meternames" },
            { "--debug", "debug" }
        };

        /// <summary>
        /// Creates an instance of <see cref="HoneycombOptions"/> from command line parameters.
        /// </summary>
        public static HoneycombOptions FromArgs(params string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args, CommandLineSwitchMap)
                .Build();
            var honeycombOptions = config.Get<HoneycombOptions>();

            var meterNames = config.GetValue<string>("meternames");
            if (!string.IsNullOrWhiteSpace(meterNames))
            {
                honeycombOptions.MeterNames = new List<string>(meterNames.Split(','));
            }

            return honeycombOptions;
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