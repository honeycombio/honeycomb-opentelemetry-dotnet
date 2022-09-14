using OpenTelemetry.Resources;
using System;
using System.Runtime.InteropServices;
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
        public static ResourceBuilder AddHoneycombAttributes(this ResourceBuilder builder)
        {
            return builder
                .AddAttributes(new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("honeycomb.distro.language", "dotnet"),
                    new KeyValuePair<string, object>("honeycomb.distro.version", GetFileVersion()),
                    new KeyValuePair<string, object>("honeycomb.distro.runtime_version",
                        Environment.Version.ToString()),
                })
                .AddRuntimeResource()
                .AddOSResource();
        }

        private static ResourceBuilder AddRuntimeResource(this ResourceBuilder builder)
        {
            var frameworkDescription = RuntimeInformation.FrameworkDescription;
            var parts = frameworkDescription.Split(' ');
            return builder
                .AddAttributes(new List<KeyValuePair<string, object>>
                {
                    // See here:
                    // https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.runtimeinformation.frameworkdescription?view=netstandard-2.0
                    // Description likely cannot be synthesized, at least not easily.
                    new KeyValuePair<string, object>("process.runtime.name", parts[0]),
                    new KeyValuePair<string, object>("process.runtime.version", parts[1])
                });
        }

        private static ResourceBuilder AddOSResource(this ResourceBuilder builder)
        {
            var osType = "unknown";
            var osName = "unknown";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                osType = "windows";
                osName = "Windows";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                osType = "linux";
                osName = "Linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                osType = "darwin";
                osName = "macOS";
            }

            var versionWithPotentialWindowsServicePack = Environment.OSVersion.VersionString.IndexOf(" ") + 1;
            return builder
                .AddAttributes(new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("os.type", osType),
                    new KeyValuePair<string, object>("os.description",
                        Environment.OSVersion.VersionString),
                    new KeyValuePair<string, object>("os.name", osName),
                    new KeyValuePair<string, object>("os.version",
                        Environment.OSVersion.VersionString.Substring(versionWithPotentialWindowsServicePack)),
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
