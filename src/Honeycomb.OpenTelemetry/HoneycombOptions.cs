using Microsoft.Extensions.Configuration;
using OpenTelemetry.Trace;
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
        /// Honeycomb dataset to store telemetry data.
        /// <para/>
        /// <b>Required</b>
        /// </summary>
        public string Dataset { get; set; }

        /// <summary>
        /// API endpoint to send telemetry data. Defaults to <see cref="DefaultEndpoint"/>.
        /// </summary>
        public string Endpoint { get; set; } = DefaultEndpoint;

        /// <summary>
        /// Sample rate for sending telemetry data. Defaults to <see cref="DefaultSampleRate"/>.
        /// <para/>
        /// See <see cref="DeterministicSampler"/> for more details on how sampling is applied.
        /// </summary>
        public uint SampleRate { get; set; } = DefaultSampleRate;

        /// <summary>
        /// Serice name used to identify application. Defaults to application assembly name.
        /// </summary>
        public string ServiceName { get; set; } = s_defaultServiceName;

        /// <summary>
        /// Service version. Defaults to application assembly information version.
        /// </summary>
        public string ServiceVersion { get; set; } = s_defaultServiceVersion;

        /// <summary>
        /// Redis connection to enable Redis instrumentation.
        /// </summary>
        public IConnectionMultiplexer RedisConnection { get; set; }

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

        /// <summary>
        /// Creates an instance of <see cref="HoneycombOptions"/> using <see cref="IConfiguration"/>.
        /// </summary>
        public static HoneycombOptions FromConfiguration(IConfiguration configuration)
        {
            const string configurationKey = "Honeycomb";
            return new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .Build()
                .GetSection(configurationKey)
                .Get<HoneycombOptions>();
        }
    }
}