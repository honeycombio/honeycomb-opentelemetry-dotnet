#if NETSTANDARD2_0_OR_GREATER

using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using System;

namespace Honeycomb.OpenTelemetry
{
    public static class HoneycombHostingExtensions
    {
        public static IServiceCollection UseHoneycomb(this IServiceCollection services, Action<HoneycombOptions> configureOptions)
        {
            var options = new HoneycombOptions();
            configureOptions?.Invoke(options);
            return services.UseHoneycomb(options);
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