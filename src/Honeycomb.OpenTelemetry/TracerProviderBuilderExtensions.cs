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

            // if serviceName is null, warn and set to default
            if (string.IsNullOrWhiteSpace(options.ServiceName)) {
                options.ServiceName = HoneycombOptions.SDefaultServiceName;
                Console.WriteLine($"WARN: {EnvironmentOptions.GetErrorMessage("service name","SERVICE_NAME")}. If left unset, this will show up in Honeycomb as unknown_service:<process_name>.");
            }

            builder
                .AddSource(options.ServiceName)
                .SetSampler(new DeterministicSampler(options.SampleRate))
                .SetResourceBuilder(
                    options.ResourceBuilder
                        .AddHoneycombAttributes()
                        .AddEnvironmentVariableDetector()
                        .AddService(serviceName: options.ServiceName, serviceVersion: options.ServiceVersion)
                )
                .AddProcessor(new BaggageSpanProcessor());

            if (!string.IsNullOrWhiteSpace(options.TracesApiKey)) {
                builder.AddOtlpExporter(otlpOptions => {
                    otlpOptions.Endpoint = new Uri(options.TracesEndpoint);
                    otlpOptions.Headers = options.GetTraceHeaders();
                });
            } else {
                Console.WriteLine($"WARN: {EnvironmentOptions.GetErrorMessage("API Key", "HONEYCOMB_API_KEY")}.");
            }

            // heads up: even if dataset is set, it will be ignored
            if (!string.IsNullOrWhiteSpace(options.TracesApiKey) & !options.IsTracesLegacyKey() & (!string.IsNullOrWhiteSpace(options.TracesDataset))) {
                if (!string.IsNullOrWhiteSpace(options.ServiceName)) {
                    Console.WriteLine($"WARN: Dataset is ignored in favor of service name. Data will be sent to service name: {options.ServiceName}");
                } else {
                    // should only get here if missing service name and dataset
                    Console.WriteLine("WARN: Dataset is ignored in favor of service name.");
                }
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
