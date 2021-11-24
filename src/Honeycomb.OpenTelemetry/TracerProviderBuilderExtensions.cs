using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;

namespace Honeycomb.OpenTelemetry
{
    /// <summary>
    /// Extension methods to configure <see cref="TracerProviderBuilder"/> to send telemetry data to Honeycomb.
    /// </summary>
    public static class TracerProviderBuilderExtensions
    {
        /// <summary>
        /// Configures the <see cref="TracerProviderBuilder"/> to send telemetry data to Honeycomb using options created from command line arguments.
        /// </summary>
        public static TracerProviderBuilder AddHoneycomb(this TracerProviderBuilder builder, string[] args)
        {
            return builder.AddHoneycomb(HoneycombOptions.FromArgs(args));
        }

        /// <summary>
        /// Configures the <see cref="TracerProviderBuilder"/> to send telemetry data to Honeycomb.
        /// </summary>
        /// <param name="builder"><see cref="TracerProviderBuilder"/> being configured.</param>
        /// <param name="configureHoneycombOptions">Action delegate that configures a <see cref="HoneycombOptions"/>.</param>
        /// <returns>The instance of <see cref="TracerProviderBuilder"/> to chain the calls.</returns>
        public static TracerProviderBuilder AddHoneycomb(this TracerProviderBuilder builder, Action<HoneycombOptions> configureHoneycombOptions = null)
        {
            var honeycombOptions = new HoneycombOptions{};
            configureHoneycombOptions?.Invoke(honeycombOptions);
            return builder.AddHoneycomb(honeycombOptions);
        }

        /// <summary>
        /// Configures the <see cref="TracerProviderBuilder"/> to send telemetry data to Honeycomb using an instance of <see cref="HoneycombOptions"/>.
        /// </summary>
        public static TracerProviderBuilder AddHoneycomb(this TracerProviderBuilder builder, HoneycombOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.ApiKey))
                throw new ArgumentException("API key cannot be empty");
            if (string.IsNullOrWhiteSpace(options.Dataset))
                throw new ArgumentException("Dataset cannot be empty");

            builder
                .AddSource(options.ServiceName)
                .SetSampler(new DeterministicSampler(options.SampleRate))
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(options.Endpoint);
                    otlpOptions.Headers = string.Format("x-honeycomb-team={0},x-honeycomb-dataset={1}", options.ApiKey, options.Dataset);
                })
                .SetResourceBuilder(
                    ResourceBuilder
                        .CreateDefault()
                        .AddHoneycombAttributes()
                        .AddEnvironmentVariableDetector()
                        .AddService(serviceName: options.ServiceName, serviceVersion: options.ServiceVersion)
                )
                .AddProcessor(new BaggageSpanProcessor());
            
            if (options.InstrumentHttpClient)
            {
                #if NET461
                    builder.AddHttpClientInstrumentation();
                #else
                    builder.AddHttpClientInstrumentation(options.ConfigureHttpClientInstrumentationOptions);
                #endif
            }
            
            if (options.InstrumentSqlClient)
            {
                builder.AddSqlClientInstrumentation(options.ConfigureSqlClientInstrumentationOptions);
            }

            if (options.InstrumentStackExchangeRedisClient)
            {
                builder.AddRedisInstrumentation(options.RedisConnection, // if null, resolved using the application IServiceProvider.
                    options.ConfigureStackExchangeRedisClientInstrumentationOptions);
            }

#if NET461
            builder.AddAspNetInstrumentation(opts =>
                opts.Enrich = (activity, eventName, _) =>
                {
                    if (eventName == "OnStartActivity")
                        foreach (KeyValuePair<string, string> entry in Baggage.Current)
                        {
                            activity.SetTag(entry.Key, entry.Value);
                        }
                });
#endif

#if NETSTANDARD2_1
            if (options.InstrumentGprcClient && options.InstrumentHttpClient) // HttpClient needs to be instrumented for GrpcClient instrumentation to work.
            {
                // See https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Instrumentation.GrpcNetClient/README.md#suppressdownstreaminstrumentation
                builder.AddGrpcClientInstrumentation(options => options.SuppressDownstreamInstrumentation = true);
            }
#endif

            return builder;
        }
    }
}