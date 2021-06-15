using System.Collections.Generic;
using System.Diagnostics;
using OpenTelemetry.Trace;
using Xunit;
using Xunit.Abstractions;

namespace Honeycomb.OpenTelemetry.Tests
{
    public class DeterministicSamplerTests
    {
        private readonly ITestOutputHelper output;

        public DeterministicSamplerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void NeverSampleRateAlwaysReturnsDropResult()
        {
            const uint sampleRate = 0;
            var sampler = new DeterministicSampler(sampleRate);

            for (int i = 0; i < 100; i++)
            {
                var traceID = ActivityTraceId.CreateRandom();
                var parameters = new SamplingParameters(
                    parentContext: new ActivityContext(
                        traceID,
                        ActivitySpanId.CreateRandom(),
                        ActivityTraceFlags.None),
                    traceId: traceID,
                    name: "Span",
                    kind: ActivityKind.Client
                );
                var result = sampler.ShouldSample(parameters);

                Assert.Equal(SamplingDecision.Drop, result.Decision);
                Assert.Collection(result.Attributes,
                    item => Assert.Equal(new KeyValuePair<string, object>("sampleRate", sampleRate), item)
                );
            }
        }

        [Fact]
        public void AlwaysSampleRateAlwaysReturnsDropResult()
        {
            const uint sampleRate = 1;
            var sampler = new DeterministicSampler(sampleRate);

            for (int i = 0; i < 100; i++)
            {
                var traceID = ActivityTraceId.CreateRandom();
                var parameters = new SamplingParameters(
                    parentContext: new ActivityContext(
                        traceID,
                        ActivitySpanId.CreateRandom(),
                        ActivityTraceFlags.None),
                    traceId: traceID,
                    name: "Span",
                    kind: ActivityKind.Client
                );
                var result = sampler.ShouldSample(parameters);

                Assert.Equal(SamplingDecision.RecordAndSample, result.Decision);
                Assert.Collection(result.Attributes,
                    item => Assert.Equal(new KeyValuePair<string, object>("sampleRate", sampleRate), item)
                );
            }
        }

        [Theory]
        [InlineData("a5a013ab53993340b648bc38ab92318d", SamplingDecision.RecordAndSample)]
        [InlineData("f30eb8bab58b954ebc2b99a27ad23ba5", SamplingDecision.RecordAndSample)]
        [InlineData("f380ceeefeb9914f831b0294c59d454e", SamplingDecision.RecordAndSample)]
        [InlineData("0875071575a9ce47b52732b4ca64ccc9", SamplingDecision.RecordAndSample)]
        [InlineData("4d7feb838b90fb4c88a4905b39460cfa", SamplingDecision.RecordAndSample)]
        [InlineData("741de0d981866e47b2207fcb9be1c207", SamplingDecision.RecordAndSample)]
        [InlineData("eda648054e388d49a80cffaef8d182bc", SamplingDecision.RecordAndSample)]
        [InlineData("ed4089963f24634dbc91cc2b3d55407c", SamplingDecision.RecordAndSample)]
        [InlineData("e06e17e6b57fc54489e0e205f060a1e5", SamplingDecision.RecordAndSample)]
        [InlineData("d58153786958be408106f7297836229f", SamplingDecision.RecordAndSample)]
        [InlineData("5a8794bd6402bd429054fa71fb58b68c", SamplingDecision.RecordAndSample)]
        [InlineData("7f09606a8908fc49a824cf2189a7e087", SamplingDecision.Drop)]
        [InlineData("25c9c9f1002576469999e69e02c0be7e", SamplingDecision.Drop)]
        [InlineData("c7f818f60d780145b4355ab0603d4d0c", SamplingDecision.Drop)]
        [InlineData("817a087bbbed1b4c96258f07d59fe006", SamplingDecision.Drop)]
        [InlineData("46cd1af9a07eaa40b00f227dea73c3fc", SamplingDecision.Drop)]
        [InlineData("384c70f32674c04ca2709c58fe066721", SamplingDecision.Drop)]
        [InlineData("209e3ba9ecc648458b722ee622106a05", SamplingDecision.Drop)]
        [InlineData("560b7b48bfd99c429bfb0d497ea260c6", SamplingDecision.Drop)]
        [InlineData("2f3467b35730064597b4d93440cdc033", SamplingDecision.Drop)]
        public void BeelineInterop(string traceID, SamplingDecision expectedDescision)
        {
            const uint sampleRate = 2;
            var sampler = new DeterministicSampler(sampleRate);

            var actiivityTraceID = ActivityTraceId.CreateFromString(traceID);
            var parameters = new SamplingParameters(
                parentContext: new ActivityContext(
                    actiivityTraceID,
                    ActivitySpanId.CreateRandom(),
                    ActivityTraceFlags.None),
                traceId: actiivityTraceID,
                name: "Span",
                kind: ActivityKind.Client
            );

            var result = sampler.ShouldSample(parameters);
            Assert.Equal(result.Decision, expectedDescision);
            Assert.Collection(result.Attributes,
                item => Assert.Equal(new KeyValuePair<string, object>("sampleRate", sampleRate), item)
            );
        }
    }
}
