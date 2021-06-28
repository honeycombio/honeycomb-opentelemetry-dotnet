using System;
using System.Collections.Generic;
using System.Reflection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Honeycomb.OpenTelemetry
{
    public class HoneycombSdkBuilder
    {
        private Uri _endpoint;
        private string _apiKey;
        private string _dataset;
        private string[] _sourceNames = {"honeycomb.opentelemetry"};
        private Sampler _sampler = new DeterministicSampler(1); // default to always sample
        internal readonly ResourceBuilder ResourceBuilder;

        public HoneycombSdkBuilder()
        {
            var options = new EnvironmentOptions(Environment.GetEnvironmentVariables());
            _apiKey = options.ApiKey;
            _dataset = options.Dataset;
            _endpoint = new Uri(options.ApiEndpoint);
            _sampler = new DeterministicSampler(options.SampleRate);

            ResourceBuilder = ResourceBuilder
                .CreateDefault()
                .AddAttributes(new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("honeycomb.distro.language", "dotnet"),
                    new KeyValuePair<string, object>("honeycomb.distro.version",
                        typeof(HoneycombSdkBuilder).Assembly
                            .GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion),
                    new KeyValuePair<string, object>("honeycomb.distro.runtime_version",
                        Environment.Version.ToString()),
                })
                .AddEnvironmentVariableDetector();

            if (!string.IsNullOrWhiteSpace(options.ServiceName))
            {
                ResourceBuilder.AddService(serviceName: options.ServiceName, serviceVersion: options.ServiceVersion);
            }
        }

        public HoneycombSdkBuilder SetEndpoint(string endpoint)
        {
            return SetEndpoint(new Uri(endpoint));
        }

        public HoneycombSdkBuilder SetEndpoint(Uri endpoint)
        {
            _endpoint = endpoint;
            return this;
        }

        public HoneycombSdkBuilder SetAPIKey(string apiKey)
        {
            _apiKey = apiKey;
            return this;
        }

        public HoneycombSdkBuilder SetDataset(string dataset)
        {
            _dataset = dataset;
            return this;
        }

        public HoneycombSdkBuilder SetSampler(Sampler sampler)
        {
            _sampler = sampler;
            return this;
        }

        public HoneycombSdkBuilder SetSampleRate(uint sampleRate)
        {
            _sampler = new DeterministicSampler(sampleRate);
            return this;
        }

        public HoneycombSdkBuilder SetServiceName(string serviceName)
        {
            ResourceBuilder.AddService(serviceName);
            return this;
        }

        public HoneycombSdkBuilder AddResourceAttributes(params KeyValuePair<string, object>[] attributes)
        {
            ResourceBuilder.AddAttributes(attributes);
            return this;
        }

        public HoneycombSdkBuilder AddResourceAttribute(string key, object value)
        {
            return AddResourceAttributes(new KeyValuePair<string, object>(key, value));
        }

        public HoneycombSdkBuilder WithSources(params string[] names)
        {
            _sourceNames = names;
            return this;
        }

        public HoneycombSdk Build()
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new ArgumentException("API key cannot be empty");
            if (string.IsNullOrWhiteSpace("dataset"))
                throw new ArgumentException("Dataset cannot be empty");

            var traceProviderBuilder = Sdk.CreateTracerProviderBuilder()
                .AddSource(_sourceNames)
                .SetSampler(_sampler)
                .SetResourceBuilder(ResourceBuilder)
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = _endpoint;
                    otlpOptions.Headers = string.Format($"x-honeycomb-team={_apiKey},x-honeycomb-dataset={_dataset}");
                })
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation();

#if NETSTANDARD2_0_OR_GREATER
            traceProviderBuilder.AddAspNetCoreInstrumentation(options => {
                options.RecordException = true;
            });
#endif

            return new HoneycombSdk(traceProviderBuilder.Build());
        }
    }
}