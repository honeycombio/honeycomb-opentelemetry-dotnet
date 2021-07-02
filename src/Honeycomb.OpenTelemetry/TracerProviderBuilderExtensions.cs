using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Reflection;

#if NETSTANDARD2_0_OR_GREATER
using Microsoft.Extensions.Configuration;
#endif

namespace Honeycomb.OpenTelemetry
{
    public static class TracerProviderBuilderExtensions
    {
#if NETSTANDARD2_0_OR_GREATER
        public static TracerProviderBuilder UseHoneycomb(this TracerProviderBuilder builder, string[] args)
        {
            return builder.UseHoneycomb(HoneycombOptions.FromArgs(args));
        }

        public static TracerProviderBuilder UseHoneycomb(this TracerProviderBuilder builder, IConfiguration configuration)
        {
            return builder.UseHoneycomb(HoneycombOptions.FromConfiguration(configuration));
        }
#endif

        public static TracerProviderBuilder UseHoneycomb(this TracerProviderBuilder builder, HoneycombOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.ApiKey))
                throw new ArgumentException("API key cannot be empty");
            if (string.IsNullOrWhiteSpace(options.Dataset))
                throw new ArgumentException("Dataset cannot be empty");

            builder
                .AddSource(options.ServiceName)
                .SetSampler(new DeterministicSampler(options.SampleRate))
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(options.Endpoint);
                    otlpOptions.Headers = string.Format($"x-honeycomb-team={options.ApiKey},x-honeycomb-dataset={options.Dataset}");
                })
                .SetResourceBuilder(
                    ResourceBuilder
                        .CreateDefault()
                        .AddAttributes(new List<KeyValuePair<string, object>>
                        {
                            new KeyValuePair<string, object>("honeycomb.distro.language", "dotnet"),
                            new KeyValuePair<string, object>("honeycomb.distro.version",
                                typeof(TracerProviderBuilderExtensions).Assembly
                                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion),
                            new KeyValuePair<string, object>("honeycomb.distro.runtime_version",
                                Environment.Version.ToString()),
                        })
                        .AddEnvironmentVariableDetector()
                        .AddService(serviceName: options.ServiceName, serviceVersion: options.ServiceVersion)
                )
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation();

            return builder;
        }
    }
}