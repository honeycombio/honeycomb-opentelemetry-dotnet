using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;

#if NET461
using System.Collections.Generic;
using OpenTelemetry;
#endif

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
        public static TracerProviderBuilder AddHoneycomb(this TracerProviderBuilder builder,
            Action<HoneycombOptions> configureHoneycombOptions = null)
        {
            var honeycombOptions = new HoneycombOptions { };
            configureHoneycombOptions?.Invoke(honeycombOptions);
            return builder.AddHoneycomb(honeycombOptions);
        }

        /// <summary>
        /// Configures the <see cref="TracerProviderBuilder"/> to send telemetry data to Honeycomb using an instance of <see cref="HoneycombOptions"/>.
        /// </summary>
        public static TracerProviderBuilder AddHoneycomb(this TracerProviderBuilder builder, HoneycombOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options), "No Honeycomb options have been set in appsettings.json, environment variables, or the command line.");
            }
            
            builder
                .SetSampler(new DeterministicSampler(options.SampleRate))
                .SetResourceBuilder(
                    ResourceBuilder
                        .CreateDefault()
                        .AddHoneycombAttributes()
                        .AddEnvironmentVariableDetector()
                        .AddService(serviceName: options.ServiceName, serviceVersion: options.ServiceVersion)
                )
                .AddProcessor(new BaggageSpanProcessor());

            if (!string.IsNullOrWhiteSpace(options.ServiceName)) {
                builder.AddSource(options.ServiceName);
            } else {
                Console.WriteLine("WARN: missing service name. If left unset, this will show up in Honeycomb as unknown_service:<process_name>.");
            }

            if (!string.IsNullOrWhiteSpace(options.TracesApiKey)) {
                String headers = $"x-honeycomb-team={options.TracesApiKey}";
                if (options.isLegacyKey()) {
                    // if the key is legacy, add dataset to the header
                    if (!string.IsNullOrWhiteSpace(options.TracesDataset)) {
                        headers += $",x-honeycomb-team={options.TracesDataset}";
                    } else {
                        // if legacy key and missing dataset, warn on missing dataset
                        Console.WriteLine("WARN: missing traces dataset");
                    }
                }
                builder.AddOtlpExporter(otlpOptions => {
                    otlpOptions.Endpoint = new Uri(options.TracesEndpoint);
                    otlpOptions.Headers = headers;
                });
            } else {
                Console.WriteLine("WARN: missing traces API key");
            }

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

            if (options.InstrumentStackExchangeRedisClient && options.RedisConnection != null)
            {
                builder.AddRedisInstrumentation(options.RedisConnection,
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
            if (options.InstrumentGrpcClient && options.InstrumentHttpClient) // HttpClient needs to be instrumented for GrpcClient instrumentation to work.
            {
                // See https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Instrumentation.GrpcNetClient/README.md#suppressdownstreaminstrumentation
                builder.AddGrpcClientInstrumentation(options => options.SuppressDownstreamInstrumentation = true);
            }
#endif

            return builder;
        }
    }
}
