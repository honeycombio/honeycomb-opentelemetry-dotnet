using System.Collections.Generic;
using System.Diagnostics;
using OpenTelemetry.Trace;
using Xunit;

namespace Honeycomb.OpenTelemetry.Tests
{
    public class DeterministicSamplerTests
    {
        [Fact]
        public void NeverSampleRateAlwaysReturnsDropResult()
        {
            var sampler = new DeterministicSampler(DeterministicSampler.NeverSample);
            for (int i = 0; i < 100; i++)
            {
                var traceID = ActivityTraceId.CreateRandom();
                var parameters = new SamplingParameters(
                    parentContext: default,
                    traceId: traceID,
                    name: "Span",
                    kind: ActivityKind.Client
                );

                var result = sampler.ShouldSample(parameters);
                Assert.Equal(SamplingDecision.Drop, result.Decision);
            }
        }

        [Fact]
        public void AlwaysSampleRateAlwaysReturnsDropResult()
        {
            var sampler = new DeterministicSampler(DeterministicSampler.AlwaysSample);
            for (int i = 0; i < 100; i++)
            {
                var traceID = ActivityTraceId.CreateRandom();
                var parameters = new SamplingParameters(
                    parentContext: default,
                    traceId: traceID,
                    name: "Span",
                    kind: ActivityKind.Client
                );

                var result = sampler.ShouldSample(parameters);
                Assert.Equal(SamplingDecision.RecordAndSample, result.Decision);
            }
        }

        [Fact]
        public void DeterministicSamplerResultVariesBasedOnTraceId()
        {
            const uint sampleRate = 2;
            var sampler = new DeterministicSampler(sampleRate);
            var count = 0;
            for (int i = 0; i < 100; i++)
            {
                var activityTraceID = ActivityTraceId.CreateRandom();
                var parameters = new SamplingParameters(
                    parentContext: default,
                    traceId: activityTraceID,
                    name: "Span",
                    kind: ActivityKind.Client
                );
                var result = sampler.ShouldSample(parameters);
                if (result.Decision == SamplingDecision.RecordAndSample) {
                    count++;
                }
            }

            Assert.True(count > 25 && count < 75);
        }

        [Fact]
        public void DeterministicSamplerAddsSampleRateAttribute()
        {
            for (int i = 0; i < 100; i++)
            {
                var sampler = new DeterministicSampler((uint) i);
                var activityTraceID = ActivityTraceId.CreateRandom();
                var parameters = new SamplingParameters(
                    parentContext: default,
                    traceId: activityTraceID,
                    name: "Span",
                    kind: ActivityKind.Client
                );

                var result = sampler.ShouldSample(parameters);
                Assert.Collection(result.Attributes,
                    item => Assert.Equal(new KeyValuePair<string, object>(DeterministicSampler.SampleRateField, (uint) i), item)
                );
            }
        }
    }
}
