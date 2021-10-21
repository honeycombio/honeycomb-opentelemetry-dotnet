using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using System.Collections.Generic;

namespace Honeycomb.OpenTelemetry
{
    /// <summary>
    /// Extension methods to configure <see cref="IServiceCollection"/> to send telemetry data to Honeycomb.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the <see cref="IServiceCollection"/> to send telemetry data to Honeycomb using an instance of <see cref="HoneycombOptions"/>.
        /// </summary>
        public static IServiceCollection AddHoneycomb(this IServiceCollection services, Action<HoneycombOptions> configureHoneycombOptions = null)
        {
#if (NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_1)
            
            var honeycombOptions = new HoneycombOptions();
            configureHoneycombOptions?.Invoke(honeycombOptions);       

            services
                .AddOpenTelemetryTracing(hostingBuilder => hostingBuilder.Configure(((serviceProvider, builder) =>
                    {
                        builder
                            .AddHoneycomb(honeycombOptions)
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
                .AddSingleton(TracerProvider.Default.GetTracer(honeycombOptions.ServiceName));
#endif
            return services;
        }
    }
}