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
        private Sampler _sampler;
        internal readonly ResourceBuilder ResourceBuilder;

        public HoneycombSdkBuilder()
        {
            var options = new EnvironmentOptions();
            _apiKey = options.ApiKey;
            _dataset = options.Dataset;
            _endpoint = new Uri(options.ApiEndpoint);
            _sampler = new DeterministicSampler(options.SampleRate);

            ResourceBuilder = ResourceBuilder
                .CreateDefault()
                .AddAttributes(new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("honeycomb.distro.language", "dotnet"),
                    new KeyValuePair<string, object>("honeycomb.distro.version", typeof(HoneycombSdkBuilder).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion),
                    new KeyValuePair<string, object>("honeycomb.distro.runtime_version", Environment.Version.ToString()),
                })
                .AddEnvironmentVariableDetector()
                .AddService(serviceName: options.ServiceName, serviceVersion: options.ServiceVersion);
        }

        public HoneycombSdkBuilder WithEndpoint(string endpoint)
        {
            return WithEndpoint(new Uri(endpoint));
        }

        public HoneycombSdkBuilder WithEndpoint(Uri endpoint)
        {
            _endpoint = endpoint;
            return this;
        }

        public HoneycombSdkBuilder WithAPIKey(string apiKey)
        {
            _apiKey = apiKey;
            return this;
        }

        public HoneycombSdkBuilder WithDataset(string dataset)
        {
            _dataset = dataset;
            return this;
        }

        public HoneycombSdkBuilder WithSampler(Sampler sampler)
        {
            _sampler = sampler;
            return this;
        }

        public HoneycombSdkBuilder WithSampleRate(uint sampleRate)
        {
            _sampler = new DeterministicSampler(sampleRate);
            return this;
        }

        public HoneycombSdkBuilder WithServiceName(string serviceName)
        {
            ResourceBuilder.AddService(serviceName);
            return this;
        }

        public HoneycombSdkBuilder WithResourceAttributes(params KeyValuePair<string, object>[] attributes)
        {
            ResourceBuilder.AddAttributes(attributes);
            return this;
        }

        public HoneycombSdkBuilder WithResourceAttribute(string key, object value)
        {
            return WithResourceAttributes(new KeyValuePair<string, object>(key, value));
        }

        public HoneycombSdk Build()
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new ArgumentException("API key cannot be empty");
            if (string.IsNullOrWhiteSpace("dataset"))
                throw new ArgumentException("Dataset cannot be empty");

            var traceProvider = Sdk.CreateTracerProviderBuilder()
                .SetSampler(_sampler)
                .SetResourceBuilder(ResourceBuilder)
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = _endpoint;
                    otlpOptions.Headers = string.Format($"x-honeycomb-team={_apiKey},x-honeycomb-dataset={_dataset}");
                })
                .Build();

            return new HoneycombSdk(traceProvider);
        }
    }
}
