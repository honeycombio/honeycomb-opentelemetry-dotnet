#if !NET462
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

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
            return services.AddHoneycomb(configuration.GetSection(HoneycombOptions.ConfigSectionName).Get<HoneycombOptions>());
        }

        /// <summary>
        /// Configures the <see cref="IServiceCollection"/> to send telemetry data to Honeycomb using an instance of <see cref="HoneycombOptions"/>.
        /// </summary>
        public static IServiceCollection AddHoneycomb(this IServiceCollection services, HoneycombOptions options)
        {
            options = options ?? new HoneycombOptions();
            services
                .AddOpenTelemetryTracing(hostingBuilder => hostingBuilder.Configure(((serviceProvider, builder) =>
                    {
                        builder
                            .AddHoneycomb(options);
                    }))
                )
                .AddSingleton(TracerProvider.Default.GetTracer(options.ServiceName))
                .AddOpenTelemetryMetrics(builder => builder.AddHoneycomb(options));
            return services;
        }
    }
}
#endif
