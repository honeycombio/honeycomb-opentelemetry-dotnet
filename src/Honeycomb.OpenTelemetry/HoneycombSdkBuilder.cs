using System;
using System.Collections.Generic;
using System.Reflection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Honeycomb.OpenTelemetry
{
    /// <summary>
    /// The Honeycomb SDK builder.
    /// </summary>
    public class HoneycombSdkBuilder
    {
        private Uri _endpoint;
        private string _apiKey;
        private string _dataset;
        private string[] _sourceNames = {"honeycomb.opentelemetry"};
        private Sampler _sampler = new DeterministicSampler(1); // default to always sample
        internal readonly ResourceBuilder ResourceBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="HoneycombSdkBuilder"/> class with default values from your environment.
        /// </summary>
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

        /// <summary>
        /// Creates a new instance of the <see cref="HoneycombSdkBuilder"/> class with a specified endpoint.
        /// </summary>
        public HoneycombSdkBuilder WithEndpoint(string endpoint)
        {
            return WithEndpoint(new Uri(endpoint));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HoneycombSdkBuilder"/> class with a specified endpoint.
        /// </summary>
        public HoneycombSdkBuilder WithEndpoint(Uri endpoint)
        {
            _endpoint = endpoint;
            return this;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HoneycombSdkBuilder"/> class with a specified API key.
        /// </summary>
        public HoneycombSdkBuilder WithAPIKey(string apiKey)
        {
            _apiKey = apiKey;
            return this;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HoneycombSdkBuilder"/> class with a specified dataset.
        /// </summary>
        public HoneycombSdkBuilder WithDataset(string dataset)
        {
            _dataset = dataset;
            return this;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HoneycombSdkBuilder"/> class with a specified sampler.
        /// </summary>
        public HoneycombSdkBuilder WithSampler(Sampler sampler)
        {
            _sampler = sampler;
            return this;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HoneycombSdkBuilder"/> class with  a specified sample rate.
        /// </summary>
        public HoneycombSdkBuilder WithSampleRate(uint sampleRate)
        {
            _sampler = new DeterministicSampler(sampleRate);
            return this;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HoneycombSdkBuilder"/> class with a specified service name.
        /// </summary>
        public HoneycombSdkBuilder WithServiceName(string serviceName)
        {
            ResourceBuilder.AddService(serviceName);
            return this;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HoneycombSdkBuilder"/> class with a specified set of resource attributes.
        /// </summary>
        public HoneycombSdkBuilder WithResourceAttributes(params KeyValuePair<string, object>[] attributes)
        {
            ResourceBuilder.AddAttributes(attributes);
            return this;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HoneycombSdkBuilder"/> class with a specified resource attribute.
        /// </summary>
        public HoneycombSdkBuilder WithResourceAttribute(string key, object value)
        {
            return WithResourceAttributes(new KeyValuePair<string, object>(key, value));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HoneycombSdkBuilder"/> class with a specified set of sources.
        /// </summary>
        public HoneycombSdkBuilder WithSources(params string[] names)
        {
            _sourceNames = names;
            return this;
        }

        /// <summary>
        /// Builds a new instance of the <see cref="HoneycombSdk"/> class from the this <see cref="HoneycombSdkBuilder"/>. 
        /// </summary>
        public HoneycombSdk Build()
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new ArgumentException("API key cannot be empty");
            if (string.IsNullOrWhiteSpace("dataset"))
                throw new ArgumentException("Dataset cannot be empty");

            var tracerProviderBuilder = Sdk.CreateTracerProviderBuilder();
            Array.ForEach(_sourceNames, source => tracerProviderBuilder.AddSource(source));
            var traceProvider = tracerProviderBuilder
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
