using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Trace;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace Honeycomb.OpenTelemetry
{
    /// <summary>
    /// Extension methods to configure <see cref="IServiceCollection"/> to send telemetry data to Honeycomb.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the <see cref="IServiceCollection"/> to send telemetry data to Honeycomb using options created using an <see cref="Action{HoneycombOptions}"/> delegate. 
        /// </summary>
        public static IServiceCollection AddHoneycomb(this IServiceCollection services, Action<HoneycombOptions> configureHoneycombOptions = null, Action<Meter> createMetrics = null)
        {
            var honeycombOptions = new HoneycombOptions();
            configureHoneycombOptions?.Invoke(honeycombOptions);
            return services.AddHoneycomb(honeycombOptions, createMetrics);
        }

        /// <summary>
        /// Configures the <see cref="IServiceCollection"/> to send telemetry data to Honeycomb using options created from an instance of <see cref="IConfiguration"/>
        /// with the <see cref="HoneycombOptions"/> contained in the configuration Section having the named stored in "HoneycombOptions.ConfigSectionName".
        /// </summary>
        public static IServiceCollection AddHoneycomb(this IServiceCollection services, IConfiguration configuration, Action<Meter> createMetrics = null)
        {
            return services.AddHoneycomb(configuration.GetSection(HoneycombOptions.ConfigSectionName).Get<HoneycombOptions>(), createMetrics);
        }

        /// <summary>
        /// Configures the <see cref="IServiceCollection"/> to send telemetry data to Honeycomb using an instance of <see cref="HoneycombOptions"/>.
        /// </summary>
        public static IServiceCollection AddHoneycomb(this IServiceCollection services, HoneycombOptions options, Action<Meter> createMetrics = null)
        {
#if (NETSTANDARD2_0_OR_GREATER)
            services
                .AddOpenTelemetryTracing(hostingBuilder => hostingBuilder.Configure(((serviceProvider, builder) =>
                    {
                        builder
                            .AddHoneycomb(options)
                            .AddAspNetCoreInstrumentation(opts =>
                            {
                                opts.RecordException = true;
                                opts.Enrich = (activity, eventName, _) =>
                                {
                                    if (eventName == "OnStartActivity")
                                        foreach (KeyValuePair<string, string> entry in Baggage.Current)
                                        {
                                            activity.SetTag(entry.Key, entry.Value);
                                        }
                                };
                            });
                    }))
                )
                .AddSingleton(TracerProvider.Default.GetTracer(options.ServiceName))
                .AddOpenTelemetryMetrics(builder => builder.AddHoneycomb(options, createMetrics));
#endif
            return services;
        }
    }
}