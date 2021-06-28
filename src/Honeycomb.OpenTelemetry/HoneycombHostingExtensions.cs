#if NETSTANDARD2_0_OR_GREATER

using Microsoft.Extensions.DependencyInjection;
using System;

namespace Honeycomb.OpenTelemetry
{
    public static class HoneycombHostingExtensions
    {
        public static IServiceCollection UseHoneycomb(this IServiceCollection services)
        {
            return services.UseHoneycomb(optoins => { });
        }

        public static IServiceCollection UseHoneycomb(this IServiceCollection services, Action<HoneycombOptions> configureBuilder)
        {
            return services.AddOpenTelemetryTracing(builder => builder.UseHoneycomb(configureBuilder));
        }
    }
}

#endif