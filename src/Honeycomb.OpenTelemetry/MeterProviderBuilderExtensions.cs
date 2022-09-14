using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using System;

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
        public static MeterProviderBuilder AddHoneycomb(this MeterProviderBuilder builder, string[] args)
        {
            return builder.AddHoneycomb(HoneycombOptions.FromArgs(args));
        }

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
                    .AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(options.MetricsEndpoint);
                        otlpOptions.Headers = options.GetMetricsHeaders();
                    });

                builder.AddMeter(options.ServiceName);
                foreach (var meterName in options.MeterNames)
                {
                    builder.AddMeter(meterName);
                }
            }

            return builder;
        }
    }
}