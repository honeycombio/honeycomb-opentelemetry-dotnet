using Microsoft.Extensions.Configuration;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Instrumentation.SqlClient;
using OpenTelemetry.Instrumentation.StackExchangeRedis;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
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

        /// <summary>
        /// Default service name if service name is not provided.
        /// </summary>
        internal static readonly string SDefaultServiceName = $"unknown_service:{System.Diagnostics.Process.GetCurrentProcess().ProcessName}";
        private static readonly string SDefaultServiceVersion = "{unknown_service_version}";

        private string _tracesApiKey;
        private string _metricsApiKey;
        private string _tracesDataset;
        private string _tracesEndpoint;
        private string _metricsEndpoint;
        private bool _enableLocalVisualizations;

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
        internal bool IsTracesLegacyKey()
        {
            // legacy key has 32 characters
            return TracesApiKey?.Length == 32;
        }

        /// <summary>
        /// Returns whether API key used to send metrics telemetry is a legacy key.
        /// </summary>
        internal bool IsMetricsLegacyKey()
        {
            // legacy key has 32 characters
            return MetricsApiKey?.Length == 32;
        }

        /// <summary>
        /// Write links to honeycomb traces as they come in
        /// </summary>
        public bool EnableLocalVisualizations
        { 
            get { return _enableLocalVisualizations; } 
            set { _enableLocalVisualizations = value; }
        }

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
        public string ServiceName { get; set; }

        /// <summary>
        /// Service version. Defaults to application assembly information version.
        /// </summary>
        public string ServiceVersion { get; set; } = SDefaultServiceVersion;

        /// <summary>
        /// Redis <see cref="IConnectionMultiplexer"/>. Set this if you aren't using a DI Container.
        /// If you're using a DI Container, then setting this isn't necessary as it will be resolved from the <see cref="IServiceProvider"/>.
        /// </summary>
        public IConnectionMultiplexer RedisConnection { get; set; }

        /// <summary>
        /// Controls whether to instrument HttpClient calls.
        /// </summary>
        public bool InstrumentHttpClient { get; set; } = true;

        /// <summary>
        /// Controls whether to instrument SqlClient calls.
        /// </summary>
        public bool InstrumentSqlClient { get; set; } = true;

        /// <summary>
        /// Controls whether to instrument GrpcClient calls when running on .NET Standard 2.1 or greater.
        /// Requires <see cref="InstrumentHttpClient" /> to be <see langword="true"/> due to the underlying implementation.
        /// </summary>
        public bool InstrumentGrpcClient { get; set; } = true;

        /// <summary>
        /// Controls whether the Stack Exchange Redis Client is instrumented.
        /// Requires that either <see cref="RedisConnection"/> is set, if you're not using a DI Container, or
        /// if you are using a DI Container, then it requires that an <see cref="IConnectionMultiplexer"/> has been registered with the <see cref="IServiceProvider"/>.
        /// </summary>
        public bool InstrumentStackExchangeRedisClient { get; set; } = true;

        /// <summary>
        /// (Optional) Options delegate to configure HttpClient instrumentation.
        /// </summary>
        public Action<HttpClientInstrumentationOptions> ConfigureHttpClientInstrumentationOptions { get; set; }

        /// <summary>
        /// (Optional) Options delegate to configure SqlClient instrumentation.
        /// </summary>
        public Action<SqlClientInstrumentationOptions> ConfigureSqlClientInstrumentationOptions { get; set; }

        /// <summary>
        /// (Optional) Options delegate to configure StackExchange.Redis instrumentation.
        /// </summary>
        public Action<StackExchangeRedisCallsInstrumentationOptions>
            ConfigureStackExchangeRedisClientInstrumentationOptions
        { get; set; }

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
            { "--service-name", "servicename" },
            { "--service-version", "serviceversion" },
            { "--instrument-http", "instrumenthttpclient" },
            { "--instrument-sql", "instrumentsqlclient" },
            { "--instrument-grpc", "instrumentgrpcclient" },
            { "--instrument-redis", "instrumentstackexchangeredisclient" },
            { "--meter-names", "meternames" }
        };

        /// <summary>
        /// Creates an instance of <see cref="HoneycombOptions"/> from command line parameters.
        /// </summary>
        public static HoneycombOptions FromArgs(params string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args, CommandLineSwitchMap)
                .Build();
            var honeycombOptions = config
                .Get<HoneycombOptions>();

            var meterNames = config.GetValue<string>("meternames");
            if (!string.IsNullOrWhiteSpace(meterNames))
            {
                honeycombOptions.MeterNames = new List<string>(meterNames.Split(','));
            }

            return honeycombOptions;
        }
    }
}