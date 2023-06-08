using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using System;

namespace Honeycomb.OpenTelemetry
{
    /// <summary>
    /// Extension methods for setting up Honeycomb in an <see cref="OpenTelemetryBuilder" />.
    /// </summary>
    public static class OpenTelemetryBuilderExtensions
    {
        /// <summary>
        /// Configures the <see cref="OpenTelemetryBuilder"/> to send telemetry data to Honeycomb.
        /// </summary>
        public static OpenTelemetryBuilder UseHoneycomb(this OpenTelemetryBuilder builder)
        {
            return builder.UseHoneycomb(o => o = new HoneycombOptions());
        }

        /// <summary>
        /// Configures the <see cref="OpenTelemetryBuilder"/> to send telemetry data to Honeycomb using an instance of <see cref="HoneycombOptions"/>.
        /// </summary>
        public static OpenTelemetryBuilder UseHoneycomb(this OpenTelemetryBuilder builder, Action<HoneycombOptions> configureHoneycomb)
        {
            if (builder.Services == null)
            {
                throw new ArgumentNullException(nameof(builder.Services));
            }

            builder.WithTracing().UseHoneycomb(configureHoneycomb);
            builder.WithMetrics().UseHoneycomb(configureHoneycomb);
            builder.WithLogging().UseHoneycomb(configureHoneycomb);

            return builder;
        }
    }
}
