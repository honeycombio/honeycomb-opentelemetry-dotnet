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
        /// Configures the <see cref="IServiceCollection"/> to send telemetry data to Honeycomb using options created from an instance of <see cref="IConfiguration"/>.
        /// </summary>
        public static IServiceCollection AddHoneycomb(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddHoneycomb(HoneycombOptions.FromConfiguration(configuration));
        }

        /// <summary>
        /// Configures the <see cref="IServiceCollection"/> to send telemetry data to Honeycomb using an instance of <see cref="HoneycombOptions"/>.
        /// </summary>
        public static IServiceCollection AddHoneycomb(this IServiceCollection services, HoneycombOptions options)
        {
#if (NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_1)
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
                        if (options.RedisConnection == null &&
                            serviceProvider.GetService(typeof(IConnectionMultiplexer)) != null)
                        {
                            builder.AddRedisInstrumentation();
                        }
                    }))
                )
                .AddSingleton(TracerProvider.Default.GetTracer(options.ServiceName));
#endif
            return services;
        }
    }
}