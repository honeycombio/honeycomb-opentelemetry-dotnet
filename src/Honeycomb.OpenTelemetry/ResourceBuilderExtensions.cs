using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Reflection;

namespace OpenTelemetry.Resources
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

        /// <summary>
        /// Determines process runtime resources and adds them to the <see cref="ResourceBuilder"/>.
        /// </summary>
        /// <remarks>
        /// See https://opentelemetry.io/docs/reference/specification/resource/semantic_conventions/process/#process-runtimes
        /// </remarks>
        private static ResourceBuilder AddRuntimeResource(this ResourceBuilder builder)
        {
            var frameworkDescription = RuntimeInformation.FrameworkDescription;

            var runtimeName = "unknown";
            var runtimeVersion = "unknown";

            if (!frameworkDescription.StartsWith(".NET"))
            {
                return builder;
            }

            if (frameworkDescription.StartsWith(".NET Core"))
            {
                runtimeName = ".NET Core";
                runtimeVersion = frameworkDescription.Substring(".NET Core".Length);
            }
            else if (frameworkDescription.StartsWith(".NET Framework"))
            {
                runtimeName = ".NET Framework";
                runtimeVersion = frameworkDescription.Substring(".NET Framework".Length);
            }
            else if (frameworkDescription.StartsWith(".NET Native"))
            {
                // I doubt we'll ever see a user match this, but it's a valid runtime, so...
                runtimeName = ".NET Native";
                runtimeVersion = frameworkDescription.Substring(".NET Native".Length);
            }
            else
            {
                runtimeName = ".NET";
                runtimeVersion = frameworkDescription.Substring(".NET".Length);
            }

            return builder
                .AddAttributes(new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("process.runtime.name", runtimeName),
                    new KeyValuePair<string, object>("process.runtime.version", runtimeVersion)
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
            var version = typeof(Honeycomb.OpenTelemetry.HoneycombOptions)
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
