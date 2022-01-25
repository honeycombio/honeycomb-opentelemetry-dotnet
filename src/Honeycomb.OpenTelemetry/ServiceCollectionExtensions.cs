using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

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
        public static IServiceCollection AddHoneycomb(this IServiceCollection services, Action<HoneycombOptions> configureHoneycombOptions = null)
        {
            var honeycombOptions = new HoneycombOptions();
            configureHoneycombOptions?.Invoke(honeycombOptions);
            return services.AddHoneycomb(honeycombOptions);
        }

        /// <summary>
        /// Configures the <see cref="IServiceCollection"/> to send telemetry data to Honeycomb using options created from an instance of <see cref="IConfiguration"/>
        /// with the <see cref="HoneycombOptions"/> contained in the configuration Section having the named stored in "HoneycombOptions.ConfigSectionName".
        /// </summary>
        public static IServiceCollection AddHoneycomb(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection(HoneycombOptions.ConfigSectionName).Get<HoneycombOptions>();

            // This is to make account for the fact that ASP.NET Core parses values from configuration
            // out as strings. Strings are fun, but Honeycomb has a type system and we should respect that instead.
            var resources = new Dictionary<string, object>();
            foreach (var kvp in options.ResourceAttributes)
            {
                var value = kvp.Value as string;
                if (value is null)
                {
                    resources.Add(kvp.Key, kvp.Value);
                }
                else
                {
                    resources.Add(kvp.Key, value.ToHoneycombType());
                }
            }

            options.ResourceAttributes = resources;

            return services.AddHoneycomb(options);
        }

        /// <summary>
        /// Configures the <see cref="IServiceCollection"/> to send telemetry data to Honeycomb using an instance of <see cref="HoneycombOptions"/>.
        /// </summary>
        public static IServiceCollection AddHoneycomb(this IServiceCollection services, HoneycombOptions options)
        {
#if (NETSTANDARD2_0_OR_GREATER)
            services
                .AddOpenTelemetryTracing(hostingBuilder => hostingBuilder.Configure(((serviceProvider, builder) =>
                    {
                        if (options.RedisConnection == null && serviceProvider.GetService<IConnectionMultiplexer>() != null)
                        {
                            options.RedisConnection = serviceProvider.GetService<IConnectionMultiplexer>();
                        }

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
                .AddOpenTelemetryMetrics(builder => builder.AddHoneycomb(options));
#endif
            return services;
        }
    }
}