using OpenTelemetry.Trace;
using System.Collections.Generic;

namespace Honeycomb
{
    /// <summary>
    /// This Sampler implementation allows for distributed sampling based on a trace ID.
    /// It accepts a sample rate N and will deterministically sample 1/N events based
    /// on the target field. Hence, two or more processes can decide whether or not to
    /// sample related events without communication.
    /// <para />
    /// - A sample rate of 0 means the Sampler will never sample.
    /// <para />
    /// - A sampler rate of 1 means the Sampler will always sample.
    /// <para />
    /// This implementation is based on the implementations (and necessarily needs to
    /// be in line with) other Honeycomb SDK implementations.
    /// </summary>
    public class DeterministicSampler : Sampler
    {
        internal const uint NeverSample = 0;
        internal const uint AlwaysSample = 1;
        internal const string SampleRateField = "SampleRate";
        private const double AlwaysSampleRatio = 1.0;
        private const double NeverSameRatio = 0.0;

        private Sampler _innerSampler;
        private readonly List<KeyValuePair<string, object>> _sampleResultAttributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeterministicSampler"/> class.
        /// </summary>
        /// <param name="sampleRate">The desired sample rate, expressed as 1/{sampleRate}.
        /// </param>
        public DeterministicSampler(uint sampleRate)
        {
            double ratio;
            switch(sampleRate)
            {
                case NeverSample:
                    ratio = NeverSameRatio;
                    break;
                case AlwaysSample:
                    ratio = AlwaysSampleRatio;
                    break;
                default:
                    ratio = AlwaysSampleRatio / sampleRate;
                    break;
            }
            _innerSampler = new ParentBasedSampler(new TraceIdRatioBasedSampler(ratio));
            _sampleResultAttributes = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(SampleRateField, sampleRate)
            };
        }

        /// <inheritdoc cref="Sampler.ShouldSample"/>
        public override SamplingResult ShouldSample(in SamplingParameters samplingParameters)
        {
            var result = _innerSampler.ShouldSample(samplingParameters);
            return new SamplingResult(
                result.Decision,
                _sampleResultAttributes
            );
        }
    }
}
