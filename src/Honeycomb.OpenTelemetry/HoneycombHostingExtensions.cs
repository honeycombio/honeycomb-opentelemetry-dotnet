#if NETSTANDARD2_0_OR_GREATER

using Microsoft.Extensions.DependencyInjection;
using System;

namespace Honeycomb.OpenTelemetry
{
    public static class HoneycombHostingExtensions
    {
        public static IServiceCollection AddHoneycombOpenTelemetryTracing(this IServiceCollection services)
        {
            return services.AddHoneycombOpenTelemetryTracing(optoins => { });
        }

        public static IServiceCollection AddHoneycombOpenTelemetryTracing(this IServiceCollection services, Action<HoneycombSdkBuilder> configureBuilder)
        {
            var builder = new HoneycombSdkBuilder();
            configureBuilder?.Invoke(builder);

            builder.Build();
            return services;
        }
    }
}

#endif