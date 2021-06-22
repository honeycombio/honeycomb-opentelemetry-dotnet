using System;
using System.Collections.Generic;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Honeycomb.OpenTelemetry
{
    public class HoneycombSdkBuilder
    {
        public const string DefaultEndpointAddress = "https://api.honeycomb.io:443";
        private Uri _endpoint = new Uri(DefaultEndpointAddress);
        private string _apiKey;
        private string _dataset;
        private Sampler _sampler = new DeterministicSampler(1); // default to always sample
        private ResourceBuilder _resourceBuilder = ResourceBuilder.CreateDefault().AddEnvironmentVariableDetector();

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
            _resourceBuilder.AddService(serviceName);
            return this;
        }

        public HoneycombSdkBuilder WithResourceAttributes(params KeyValuePair<string, object>[] attributes)
        {
            _resourceBuilder.AddAttributes(attributes);
            return this;
        }

        public HoneycombSdk Build()
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new ArgumentException("API key cannot be empty");
            if (string.IsNullOrWhiteSpace("dataset"))
                throw new ArgumentException("Dataset cannot be empty");

            var sdk = Sdk.CreateTracerProviderBuilder()
                .SetSampler(_sampler)
                .SetResourceBuilder(_resourceBuilder)
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = _endpoint;
                    otlpOptions.Headers = string.Format($"x-honeycomb-team={_apiKey},x-honeycomb-dataset={_dataset}");
                })
                // TODO: Add custom HNY env var Decector:
                // https://github.com/open-telemetry/opentelemetry-dotnet/blob/6b7f2dd77cf9d37260a853fcc95f7b77e296065d/src/OpenTelemetry/Resources/IResourceDetector.cs
                .Build();

            return new HoneycombSdk(sdk);
        }
    }
}
