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
        /// Configures the <see cref="IServiceCollection"/> to send telemetry data to Honycomb using options created from an instance of <see cref="IConfiguration"/>.
        /// </summary>
        public static IServiceCollection UseHoneycomb(this IServiceCollection services, IConfiguration configuration)
        {
            return services.UseHoneycomb(HoneycombOptions.FromConfiguration(configuration));
        }

        /// <summary>
        /// Configures the <see cref="IServiceCollection"/> to send telemetry data to Honycomb using an instance of <see cref="HoneycombOptions"/>.
        /// </summary>
        public static IServiceCollection UseHoneycomb(this IServiceCollection services, HoneycombOptions options)
        {
            return services
                .AddOpenTelemetryTracing(builder => builder
                    .UseHoneycomb(options)
                    .AddAspNetCoreInstrumentation(opts => {
                        opts.RecordException = true;
                    })
                )
                .AddSingleton(TracerProvider.Default.GetTracer(options.ServiceName));
        }
    }
}
