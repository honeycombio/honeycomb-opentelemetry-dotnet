using System;
using OpenTelemetry.Trace;

namespace Honeycomb.OpenTelemetry
{
    public class HoneycombSdk : IDisposable
    {
        private TracerProvider sdk;

        public HoneycombSdk(TracerProvider sdk)
        {
            this.sdk = sdk;
        }

        public void Dispose()
        {
            ((IDisposable)sdk).Dispose();
        }
    }
}