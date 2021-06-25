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
        public const string DefaultEndpointAddress = "https://api.honeycomb.io:443";
        private Uri _endpoint = new Uri(DefaultEndpointAddress);
        private string _apiKey;
        private string _dataset;
        private string[] _sourceNames = {"honeycomb.opentelemetry"};
        private Sampler _sampler = new DeterministicSampler(1); // default to always sample
        internal readonly ResourceBuilder ResourceBuilder;

        // private IConnection _redisConnection;

        public HoneycombSdkBuilder()
        {
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

        // public HoneycombSdkBuilder WithRedisConnection(IRedisConnection conn)
        // {
        //     _redisConnection = conn;
        //     return this;
        // }

        public HoneycombSdkBuilder WithSources(string[] names)
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

            TracerProviderBuilder tracerProviderBuilder = Sdk.CreateTracerProviderBuilder();
            Array.ForEach(_sourceNames, source => tracerProviderBuilder.AddSource(source));
            var traceProviderBuilder = tracerProviderBuilder
                .SetSampler(_sampler)
                .SetResourceBuilder(ResourceBuilder)
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = _endpoint;
                    otlpOptions.Headers = string.Format($"x-honeycomb-team={_apiKey},x-honeycomb-dataset={_dataset}");
                })
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation();

#if NETSTANDARD2_0
            traceProviderBuilder.AddAspNetCoreInstrumentation();
            traceProviderBuilder.AddRedisInstrumentation(null); // TODO: still requires connection to instrument with
#elif NETSTANDARD2_1
            traceProviderBuilder.AddGrpcClientInstrumentation();
#endif

            return new HoneycombSdk(traceProviderBuilder.Build());
        }
    }
}