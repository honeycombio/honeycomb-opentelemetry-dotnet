using System;
using OpenTelemetry.Trace;

namespace Honeycomb.OpenTelemetry
{
    public class HoneycombSdk : IDisposable
    {
        private TracerProvider _sdk;

        public HoneycombSdk(TracerProvider sdk)
        {
            _sdk = sdk;
        }

        public void Dispose()
        {
            ((IDisposable)_sdk).Dispose();
        }
    }
}