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
        private Uri endpoint = new Uri(DefaultEndpointAddress);
        private string apiKey;
        private string dataset;
        private Sampler sampler;
        private ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault().AddEnvironmentVariableDetector();

        public HoneycombSdkBuilder WithEndpoint(string endpoint)
        {
            return WithEndpoint(new Uri(endpoint));
        }

        public HoneycombSdkBuilder WithEndpoint(Uri endpoint)
        {
            this.endpoint = endpoint;
            return this;
        }

        public HoneycombSdkBuilder WithAPIKey(string apiKey)
        {
            this.apiKey = apiKey;
            return this;
        }

        public HoneycombSdkBuilder WithDataset(string dataset)
        {
            this.dataset = dataset;
            return this;
        }

        public HoneycombSdkBuilder WithSampler(Sampler sampler)
        {
            this.sampler = sampler;
            return this;
        }

        public HoneycombSdkBuilder WithServiceName(string serviceName)
        {
            resourceBuilder.AddService(serviceName);
            return this;
        }

        public HoneycombSdkBuilder WithResourceAttribute(params KeyValuePair<string, object>[] attributes)
        {
            resourceBuilder.AddAttributes(attributes);
            return this;
        }

        public HoneycombSdk Build()
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("API key cannot be empty");
            if (string.IsNullOrWhiteSpace("dataset"))
                throw new ArgumentException("Datast cannot be empty");

            var sdk = Sdk.CreateTracerProviderBuilder()
                .SetSampler(sampler)
                .SetResourceBuilder(resourceBuilder)
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = endpoint;
                    otlpOptions.Headers = string.Format($"x-honeycomb-team={apiKey},x-honeycomb-dataset={dataset}");
                })
                // TODO: Add custom HNY env var Decector:
                // https://github.com/open-telemetry/opentelemetry-dotnet/blob/6b7f2dd77cf9d37260a853fcc95f7b77e296065d/src/OpenTelemetry/Resources/IResourceDetector.cs
                .Build();

            return new HoneycombSdk(sdk);
        }
    }
}