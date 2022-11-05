using Honeycomb.OpenTelemetry;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Extension methods for <see cref="IConfiguration"/> to help configure Honeycomb with OpenTelemetry.
    /// </summary>
    public static class ConfigurationInterfaceExtensions
    {
        /// <summary>
        /// Attempts to retrieve an instance of <see cref="HoneycombOptions"/> used to configure the OpenTelemetry SDK.
        /// </summary>
        public static HoneycombOptions GetHoneycombOptions(this IConfiguration configuration)
        {
            return configuration
                .GetSection(HoneycombOptions.ConfigSectionName)
                .Get<HoneycombOptions>();
        }
    }
}
