using Honeycomb.OpenTelemetry;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Extension methods for <see cref="ConfigurationManager"/> to help configure Honeycomb with OpenTelemetry.
    /// </summary>
    public static class ConfigurationManagerExtensions
    {
        /// <summary>
        /// Attempts to retrieve an instance of <see cref="HoneycombOptions"/> used to configre the OpenTelemetry SDK.
        /// </summary>
        public static HoneycombOptions GetHoneycombOptions(this ConfigurationManager builder)
        {
            return builder
                .GetSection(HoneycombOptions.ConfigSectionName)
                .Get<HoneycombOptions>();;
        }
    }
}
