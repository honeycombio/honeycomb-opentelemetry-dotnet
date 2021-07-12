using Microsoft.Extensions.Configuration;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Honeycomb.OpenTelemetry
{
    /// <summary>
    /// Extension methods to configure <see cref="TracerProviderBuilder"/> to send telemetry data to Honeycomb.
    /// </summary>
    public static class TracerProviderBuilderExtensions
    {
        /// <summary>
        /// Configures the <see cref="TracerProviderBuilder"/> to send telemetry data to Honycomb using options created from command line arguments.
        /// </summary>
        public static TracerProviderBuilder UseHoneycomb(this TracerProviderBuilder builder, string[] args)
        {
            return builder.UseHoneycomb(HoneycombOptions.FromArgs(args));
        }

        /// <summary>
        /// Configures the <see cref="TracerProviderBuilder"/> to send telemetry data to Honycomb using options created from an instance of <see cref="IConfiguration"/>.
        /// </summary>
        public static TracerProviderBuilder UseHoneycomb(this TracerProviderBuilder builder, IConfiguration configuration)
        {
            return builder.UseHoneycomb(HoneycombOptions.FromConfiguration(configuration));
        }

        /// <summary>
        /// Configures the <see cref="TracerProviderBuilder"/> to send telemetry data to Honycomb using an instance of <see cref="HoneycombOptions"/>.
        /// </summary>
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