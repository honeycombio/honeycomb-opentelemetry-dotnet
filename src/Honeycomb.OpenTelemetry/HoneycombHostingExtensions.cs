#if NETSTANDARD2_0_OR_GREATER

using Microsoft.Extensions.DependencyInjection;
using System;

namespace Honeycomb.OpenTelemetry
{
    public static class HoneycombHostingExtensions
    {
        public static IServiceCollection AddHoneycombOpenTemeletry(this IServiceCollection services)
        {
            return services.AddHoneycombOpenTemeletry(optoins => { });
        }

        public static IServiceCollection AddHoneycombOpenTemeletry(this IServiceCollection services, Action<HoneycombSdkBuilder> configureBuilder)
        {
            var builder = new HoneycombSdkBuilder();
            configureBuilder?.Invoke(builder);

            builder.Build();
            return services;
        }

        public class HoneycombOptions
        {
            public string ApiKey { get; set; }
            public string Dataset { get; set; }
        }
    }
}

#endif