#if NETSTANDARD2_0_OR_GREATER

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

namespace Honeycomb.OpenTelemetry
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseHoneycomb(this IServiceCollection services, IConfiguration configuration)
        {
            return services.UseHoneycomb(HoneycombOptions.FromConfiguration(configuration));
        }

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

#endif
