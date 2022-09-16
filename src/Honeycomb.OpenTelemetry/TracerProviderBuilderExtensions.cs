using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;

#if NET462
using System.Collections.Generic;
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
        /// Configures the <see cref="TracerProviderBuilder"/> to send telemetry data to Honeycomb.
        /// </summary>
        /// <param name="builder"><see cref="TracerProviderBuilder"/> being configured.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> to configure with</param>
        /// <returns>The instance of <see cref="TracerProviderBuilder"/> to chain the calls.</returns>
        public static TracerProviderBuilder AddHoneycomb(this TracerProviderBuilder builder, IConfiguration configuration)
        {
            return builder.AddHoneycomb(configuration.GetSection(HoneycombOptions.ConfigSectionName).Get<HoneycombOptions>());
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

            // TODO: Add support for other environment variables
            var environmentOptions = new EnvironmentOptions(Environment.GetEnvironmentVariables());

            // if service name set in environment, prioritize it
            if (!string.IsNullOrWhiteSpace(environmentOptions.ServiceName))
            {
                options.ServiceName = environmentOptions.ServiceName;
            }

            // if serviceName is null, warn and set to default
            if (string.IsNullOrWhiteSpace(options.ServiceName))
            {
                options.ServiceName = HoneycombOptions.SDefaultServiceName;
                Console.WriteLine($"WARN: {EnvironmentOptions.GetErrorMessage("service name", "OTEL_SERVICE_NAME")}. If left unset, this will show up in Honeycomb as unknown_service:<process_name>.");
            }

            builder
                .AddSource(options.ServiceName)
                .SetResourceBuilder(
                    options.ResourceBuilder
                        .AddHoneycombAttributes()
                        .AddService(serviceName: options.ServiceName, serviceVersion: options.ServiceVersion)
                        .AddEnvironmentVariableDetector()
                );

            if (options.AddDeterministicSampler)
            {
                builder.AddDeterministicSampler(options.SampleRate);
            }

            if (options.AddBaggageSpanProcessor)
            {
                builder.AddBaggageSpanProcessor();
            }

            if (!string.IsNullOrWhiteSpace(options.TracesApiKey))
            {
                builder.AddHoneycombOtlpExporter(options.TracesApiKey, options.TracesDataset, options.TracesEndpoint);
            }
            else
            {
                Console.WriteLine($"WARN: {EnvironmentOptions.GetErrorMessage("API Key", "HONEYCOMB_API_KEY")}.");
            }

            if (options.EnableLocalVisualizations)
            {
                builder.AddProcessor(new SimpleActivityExportProcessor(new ConsoleTraceLinkExporter(options)));
            }

            // heads up: even if dataset is set, it will be ignored
            if (!string.IsNullOrWhiteSpace(options.TracesApiKey) & !options.IsTracesLegacyKey() & (!string.IsNullOrWhiteSpace(options.TracesDataset)))
            {
                if (!string.IsNullOrWhiteSpace(options.ServiceName))
                {
                    Console.WriteLine($"WARN: Dataset is ignored in favor of service name. Data will be sent to service name: {options.ServiceName}");
                }
                else
                {
                    // should only get here if missing service name and dataset
                    Console.WriteLine("WARN: Dataset is ignored in favor of service name.");
                }
            }

            return builder;
        }

        /// <summary>
        /// Configures the <see cref="TracerProviderBuilder"/> to add the <see cref="BaggageSpanProcessor"/> span processor.
        /// </summary>
        public static TracerProviderBuilder AddBaggageSpanProcessor(this TracerProviderBuilder builder)
        {
            return builder.AddProcessor(new BaggageSpanProcessor());
        }

        /// <summary>
        /// Configures the <see cref="TracerProviderBuilder"/> to add the <see cref="DeterministicSampler"/> trace sampler.
        /// </summary>
        public static TracerProviderBuilder AddDeterministicSampler(this TracerProviderBuilder builder, uint sampleRate)
        {
            return builder.SetSampler(new DeterministicSampler(sampleRate));
        }

        /// <summary>
        /// Configures the <see cref="TracerProviderBuilder"/> with an OTLP exporter that sends trace telemetry to Honeycomb.
        /// </summary>
        public static TracerProviderBuilder AddHoneycombOtlpExporter(this TracerProviderBuilder builder, string apikey, string dataset = null, string endpoint = null)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                endpoint = HoneycombOptions.DefaultEndpoint;
            }

            return builder.AddOtlpExporter(otlpOptions =>
            {
                otlpOptions.Endpoint = new Uri(endpoint);
                otlpOptions.Headers = HoneycombOptions.GetTraceHeaders(apikey, dataset);
            });
        }
    }
}
