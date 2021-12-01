using System;
using Microsoft.Extensions.Configuration;
using OpenTelemetry.Trace;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Instrumentation.SqlClient;
using OpenTelemetry.Instrumentation.StackExchangeRedis;
using StackExchange.Redis;
using System.Reflection;
using System.Collections.Generic;

namespace Honeycomb.OpenTelemetry
{
    /// <summary>
    /// Configuration options used to configure <see cref="TracerProviderBuilder"/> to send telemetry data to Honeycomb.
    /// </summary>
    public class HoneycombOptions
    {
        private static readonly string s_defaultServiceName = "{unknown_service_name}";
        private static readonly string s_defaultServiceVersion = "{unknown_service_version}";
       
        private string _tracesApiKey;
        private string _metricsApiKey;
        private string _tracesDataset;
        private string _metricsDataset;
        private string _tracesEndpoint;
        private string _metricsEndpoint;

        static HoneycombOptions()
        {
            // This works for everything other than ASP.NET (non-core) web apps
            // because they are loaded from an unmanaged COM source so
            // assembly.GetEntryAssembly() returns null
            var assembly = Assembly.GetEntryAssembly();

#if NET461
            // inspired from https://stackoverflow.com/a/6754205
            // try to load the current HTTPContext and work out the assembly name & version
            if (assembly == null && System.Web.HttpContext.Current?.ApplicationInstance != null)
            {
                var type = System.Web.HttpContext.Current.ApplicationInstance.GetType();
                while (type != null && type.Namespace == "ASP")
                {
                    type = type.BaseType;
                }

                assembly = type?.Assembly;
            }
#endif
            if (assembly != null)
            {
                s_defaultServiceName = assembly.GetName().Name;
                s_defaultServiceVersion =
                    assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
                    assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
            }
        }

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
        /// <b>Required</b>
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// API key used to send trace telemtry data to Honeycomb. Defaults to <see cref="ApiKey"/>.
        /// </summary>
        public string TracesApiKey
        {
            get { return _tracesApiKey ?? ApiKey; }
            set { _tracesApiKey = value; }
        }

        /// <summary>
        /// API key used to send metrics telemtry data to Honeycomb. Defaults to <see cref="ApiKey"/>.
        /// </summary>
        public string MetricsApiKey
        {
            get { return _metricsApiKey ?? ApiKey; }
            set { _metricsApiKey = value; }
        }

        /// <summary>
        /// Honeycomb dataset to store telemetry data.
        /// <para/>
        /// <b>Required</b>
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
        public string MetricsDataset
        {
            get { return _metricsDataset; }
            set { _metricsDataset = value; }
        }

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
        /// Service name used to identify application. Defaults to application assembly name.
        /// </summary>
        public string ServiceName { get; set; } = s_defaultServiceName;

        /// <summary>
        /// Service version. Defaults to application assembly information version.
        /// </summary>
        public string ServiceVersion { get; set; } = s_defaultServiceVersion;

        /// <summary>
        /// Redis IConnectionMultiplexer; set this if you aren't using a DI Container.
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
        public bool InstrumentGprcClient { get; set; } = true;

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
        public Action<StackExchangeRedisCallsInstrumentationOptions> ConfigureStackExchangeRedisClientInstrumentationOptions { get; set; }

        private static Dictionary<string, string> CommandLineSwitchMap = new Dictionary<string, string>
        {
            {"--honeycomb-apikey", "apikey"},
            {"--honeycomb-dataset", "dataset"},
            {"--honeycomb-endpoint", "endpoint"},
            {"--honeycomb-samplerate", "samplerate"},
            {"--service-name", "servicename"},
            {"--service-version", "serviceversion"}
        };

        /// <summary>
        /// Creates an instance of <see cref="HoneycombOptions"/> from command line parameters.
        /// </summary>
        public static HoneycombOptions FromArgs(params string[] args)
        {
            return new ConfigurationBuilder()
                .AddCommandLine(args, CommandLineSwitchMap)
                .Build()
                .Get<HoneycombOptions>();
        }
    }
}