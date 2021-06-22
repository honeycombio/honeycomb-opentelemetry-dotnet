using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using OpenTelemetry.Trace;

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
        private const uint NeverSample = 0;
        private const uint AlwaysSample = 1;
        private readonly SamplingResult NeverSampleResult;
        private readonly SamplingResult AlwaysSampleResult;
        private readonly List<KeyValuePair<string, object>> SampleResultAttributes;        
        private readonly uint UpperBound;
        private readonly uint SampleRate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeterministicSampler"/> class.
        /// </summary>
        /// <param name="sampleRate">The desired sample rate, expressed as 1/{sampleRate}.
        /// </param>
        public DeterministicSampler(uint sampleRate)
        {
            SampleRate = sampleRate;
            UpperBound = sampleRate == 0 ? 0 : uint.MaxValue / sampleRate;
            SampleResultAttributes = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("sampleRate", SampleRate)
            };
            NeverSampleResult = new SamplingResult(SamplingDecision.Drop, SampleResultAttributes);
            AlwaysSampleResult = new SamplingResult(SamplingDecision.RecordAndSample, SampleResultAttributes);
        }

        /// <inheritdoc cref="Sampler.ShouldSample"/>
        public override SamplingResult ShouldSample(in SamplingParameters samplingParameters)
        {
            switch (SampleRate)
            {
                case NeverSample:
                    return NeverSampleResult;
                case AlwaysSample:
                    return AlwaysSampleResult;
                default:
                    using (var sha = SHA1.Create())
                    {
                        // get trace ID as bytes
                        var bytes = Encoding.UTF8.GetBytes(samplingParameters.TraceId.ToHexString());

                        // compute SH1 hash
                        var hash = sha.ComputeHash(bytes);

                        // Take first four bytes as uint to determine trace sample rate
                        var determinant = Convert.ToUInt32(BitConverter.ToString(hash, 0, 4).Replace("-", string.Empty), 16);

                        // calculate decision and return with attributes
                        return new SamplingResult(
                            determinant <= UpperBound ? SamplingDecision.RecordAndSample : SamplingDecision.Drop, 
                            SampleResultAttributes
                        );
                    }
            }
        }
    }
}
