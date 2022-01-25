using OpenTelemetry.Resources;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Honeycomb.OpenTelemetry
{
    /// <summary>
    /// Extension methods to configure <see cref="ResourceBuilder"/> with Honeycomb distro attributes.
    /// </summary>
    public static class ResourceBuilderExtensions
    {
        /// <summary>
        /// Configures the <see cref="ResourceBuilder"/> with Honeycomb attributes.
        /// </summary>
        public static ResourceBuilder AddResourceAttributes(this ResourceBuilder builder, HoneycombOptions options)
        {
            return builder
                .AddAttributes(options.ResourceAttributes)
                .AddAttributes(new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("honeycomb.distro.language", "dotnet"),
                    new KeyValuePair<string, object>("honeycomb.distro.version", GetFileVersion()),
                    new KeyValuePair<string, object>("honeycomb.distro.runtime_version",
                        Environment.Version.ToString()),
                });
        }

        private static string GetFileVersion()
        {
            var version = typeof(ResourceBuilderExtensions)
                .Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;

            // AssemblyInformationalVersionAttribute may include the latest commit hash in
            // the form `{version_prefix}{version_suffix}+{commit_hash}`.
            // We should trim the hash if present to just leave the version prefix and suffix
            var i = version.IndexOf("+");
            return i > 0 
                ? version.Substring(0, i)
                : version;
        }
    }
}
