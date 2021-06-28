using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Honeycomb.OpenTelemetry
{
    public static class TracerProviderBuilderExtensions
    {
        public static TracerProviderBuilder UseHoneycomb(this TracerProviderBuilder builder)
        {
            return builder.UseHoneycomb(opt => {});
        }

        public static TracerProviderBuilder UseHoneycomb(this TracerProviderBuilder builder, Action<HoneycombOptions> configureOptions)
        {
            //TODO: fix options handling from env vars and json config file
            var options = new HoneycombOptions();
            configureOptions?.Invoke(options);

            var resourceBuilder = ResourceBuilder
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
                .AddEnvironmentVariableDetector();

            if (!string.IsNullOrWhiteSpace(options.ServiceName))
            {
                resourceBuilder.AddService(serviceName: options.ServiceName, serviceVersion: options.ServiceVersion);
            }

            builder
                .SetSampler(new DeterministicSampler(options.SampleRate))
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation()
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(options.ApiEndpoint);
                    otlpOptions.Headers = string.Format($"x-honeycomb-team={options.ApiKey},x-honeycomb-dataset={options.Dataset}");
                })
                .AddSource(options.ServiceName)
                .SetResourceBuilder(resourceBuilder);

#if NETSTANDARD2_0_OR_GREATER
            builder.AddAspNetCoreInstrumentation(opts => {
                opts.RecordException = true;
            });
#endif

            return builder;
        }
    }
}