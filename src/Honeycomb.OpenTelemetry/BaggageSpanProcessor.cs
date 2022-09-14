using OpenTelemetry;
using System.Collections.Generic;
using System.Diagnostics;

namespace Honeycomb.OpenTelemetry
{
    /// <summary>
    /// Span processor that adds <see cref="Baggage"/> fields to every new span
    /// </summary>
    public class BaggageSpanProcessor : BaseProcessor<Activity>
    {
        /// <inheritdoc />
        public override void OnStart(Activity activity)
        {
            foreach (var entry in Baggage.Current)
            {
                activity.SetTag(entry.Key, entry.Value);
            }
        }
    }
}