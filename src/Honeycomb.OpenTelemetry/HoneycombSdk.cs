using System;
using OpenTelemetry.Trace;

namespace Honeycomb.OpenTelemetry
{
    /// <summary>
    /// Represents the Honeycomb SDK. 
    /// </summary>
    public class HoneycombSdk : IDisposable
    {
        private TracerProvider _sdk;

        /// <summary>
        /// Initializes a new instance of the <see cref="HoneycombSdk"/> class from a given <see cref="TracerProvider"/>.
        /// </summary>
        public HoneycombSdk(TracerProvider sdk)
        {
            _sdk = sdk;
        }

        /// <summary>
        /// Disposes this SDK.
        /// </summary>
        public void Dispose()
        {
            ((IDisposable)_sdk).Dispose();
        }
    }
}