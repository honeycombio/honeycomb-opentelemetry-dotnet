using Honeycomb.OpenTelemetry;
using OpenTelemetry.Resources;
using System;

namespace OpenTelemetry.Metrics
{
    /// <summary>
    /// Extension methods to configure <see cref="MeterProviderBuilder"/> to send metrics telemetry data to Honeycomb.
    /// </summary>
    public static class MeterProviderBuilderExtensions
    {
        /// <summary>
        /// Configures the <see cref="MeterProviderBuilder"/> to send metrics telemetry data to Honeycomb.
        /// </summary>
        public static MeterProviderBuilder AddHoneycomb(this MeterProviderBuilder builder,
            Action<HoneycombOptions> configureHoneycombOptions = null)
        {
            var honeycombOptions = new HoneycombOptions();
            configureHoneycombOptions?.Invoke(honeycombOptions);
            return builder.AddHoneycomb(honeycombOptions);
        }

        /// <summary>
        /// Configures the <see cref="MeterProviderBuilder"/> to send metrics telemetry data to Honeycomb using an instance of <see cref="HoneycombOptions"/>.
        /// </summary>
        public static MeterProviderBuilder AddHoneycomb(this MeterProviderBuilder builder, HoneycombOptions options)
        {
            options.ApplyEnvironmentOptions(new EnvironmentOptions(Environment.GetEnvironmentVariables()));

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options), "No Honeycomb options have been set in appsettings.json, environment variables, or the command line.");
            }

            // only enable metrics if a metrics dataset is set
            if (!string.IsNullOrWhiteSpace(options.MetricsDataset))
            {
                if (string.IsNullOrWhiteSpace(options.MetricsApiKey))
                {
                    Console.WriteLine("WARN: missing metrics API key");
                }

                builder
                    .SetResourceBuilder(
                        ResourceBuilder
                            .CreateDefault()
                            .AddHoneycombAttributes()
                            .AddEnvironmentVariableDetector()
                            .AddService(serviceName: options.ServiceName, serviceVersion: options.ServiceVersion)
                    )
                    .AddHoneycombOtlpExporter(options.GetMetricsApiKey(), options.MetricsDataset, options.GetMetricsEndpoint());

                builder.AddMeter(options.MetricsDataset);
                foreach (var meterName in options.MeterNames)
                {
                    builder.AddMeter(meterName);
                }

                if (options.Debug)
                {
                    builder.AddConsoleExporter();
                }
            }

            return builder;
        }

        /// <summary>
        /// Configures the <see cref="MeterProviderBuilder"/> with an OTLP exporter that sends metrics telemetry to Honeycomb.
        /// </summary>
        public static MeterProviderBuilder AddHoneycombOtlpExporter(this MeterProviderBuilder builder, string apikey, string dataset = null, string endpoint = null)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                endpoint = HoneycombOptions.DefaultEndpoint;
            }

            return builder.AddOtlpExporter(otlpOptions =>
            {
                otlpOptions.Endpoint = new Uri(endpoint);
                otlpOptions.Headers = HoneycombOptions.GetMetricsHeaders(apikey, dataset);
            });
        }
    }
}