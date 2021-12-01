using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using System;
using System.Diagnostics.Metrics;

namespace Honeycomb.OpenTelemetry
{
    /// <summary>
    /// Extension methods to configure <see cref="MeterProviderBuilder"/> to send metrics telemetry data to Honeycomb.
    /// </summary>
    public static class MeterProviderBuilderExtensions
    {
        /// <summary>
        /// Configures the <see cref="MeterProviderBuilder"/> to send metrics telemetry data to Honeycomb using options created from command line arguments.
        /// </summary>
        public static MeterProviderBuilder AddHoneycomb(this MeterProviderBuilder builder, string[] args, Action<Meter> createMetrics = null)
        {
            return builder.AddHoneycomb(HoneycombOptions.FromArgs(args), createMetrics);
        }

        /// <summary>
        /// Configures the <see cref="MeterProviderBuilder"/> to send metrics telemetry data to Honeycomb.
        /// </summary>
        public static MeterProviderBuilder AddHoneycomb(this MeterProviderBuilder builder, Action<HoneycombOptions> configureHoneycombOptions = null, Action<Meter> createMetrics = null)
        {
            var honeycombOptions = new HoneycombOptions();
            configureHoneycombOptions?.Invoke(honeycombOptions);
            return builder.AddHoneycomb(honeycombOptions, createMetrics);
        }

        /// <summary>
        /// Configures the <see cref="MeterProviderBuilder"/> to send metrics telemetry data to Honeycomb using an instance of <see cref="HoneycombOptions"/>.
        /// </summary>
        public static MeterProviderBuilder AddHoneycomb(this MeterProviderBuilder builder, HoneycombOptions options, Action<Meter> createMetrics = null)
        {
            // only enable metrics if a metrics dataset is set
            if (!string.IsNullOrWhiteSpace(options.MetricsDataset))
            {
                Meter meter = new Meter(options.ServiceName);

                builder
                    .AddMeter(meter.Name)
                    .SetResourceBuilder(
                        ResourceBuilder
                            .CreateDefault()
                            .AddHoneycombAttributes()
                            .AddEnvironmentVariableDetector()
                            .AddService(serviceName: options.ServiceName, serviceVersion: options.ServiceVersion)
                    )
                    .AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(options.MetricsEndpoint);
                        otlpOptions.Headers = $"x-honeycomb-team={options.MetricsApiKey},x-honeycomb-dataset={options.MetricsDataset}";
                    });

                createMetrics?.Invoke(meter);
            }
            return builder;
        }
    }
}