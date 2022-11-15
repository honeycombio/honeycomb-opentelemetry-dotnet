using OpenTelemetry.Instrumentation.AspNetCore;

namespace OpenTelemetry.Trace
{
    /// <summary>
    /// Extension methods to simplify registering of ASP.NET Core request instrumentation.
    /// </summary>
    public static class TracerProviderBuilderExtensions
    {
        /// <summary>
        /// Enables the incoming requests automatic data collection for ASP.NET Core and adds current trace context baggage as
        /// attributes to created spans.
        /// </summary>
        /// <param name="builder"><see cref="TracerProviderBuilder"/> being configured.</param>
        /// <returns>The instance of <see cref="TracerProviderBuilder"/> to chain the calls.</returns>
        public static TracerProviderBuilder AddAspNetCoreInstrumentationWithBaggage(this TracerProviderBuilder builder)
        {
            return builder.AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.EnrichWithBaggage();
                }
            );
        }

        /// <summary>
        /// Configures the AspNetCore instrumentation to adds current trace context baggage as attributes to created spans.
        /// </summary>
        /// <param name="options"><see cref="AspNetCoreInstrumentationOptions"/> being configured.</param>
        public static void EnrichWithBaggage(this AspNetCoreInstrumentationOptions options)
        {
            options.EnrichWithHttpRequest = (activity, request) =>
            {
                foreach (var entry in Baggage.Current)
                {
                    activity.SetTag(entry.Key, entry.Value);
                }
            };
            options.EnrichWithHttpResponse = (activity, response) =>
            {
                foreach (var entry in Baggage.Current)
                {
                    activity.SetTag(entry.Key, entry.Value);
                }
            };
        }
    }
}
