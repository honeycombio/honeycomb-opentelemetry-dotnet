using System;
using System.Collections.Generic;

namespace Honeycomb.OpenTelemetry {
    internal static class HoneycombOptionsExtensions {
        private const string OtlpVersion = "0.16.0";

        internal static string GetTraceHeaders(this HoneycombOptions options) {
            var headers = new List<string>
            {
                $"x-otlp-version={OtlpVersion}",
                $"x-honeycomb-team={options.TracesApiKey}"
            };
            if (options.IsTracesLegacyKey())
            {
                // if the key is legacy, add dataset to the header
                if (!string.IsNullOrWhiteSpace(options.TracesDataset))
                {
                    headers.Add($"x-honeycomb-dataset={options.TracesDataset}");
                }
                else
                {
                    // if legacy key and missing dataset, warn on missing dataset
                    Console.WriteLine($"WARN: {EnvironmentOptions.GetErrorMessage("dataset", "HONEYCOMB_DATASET")}.");
                }
            }
            return string.Join(",", headers);
        }

        internal static string GetMetricsHeaders(this HoneycombOptions options) {
            var headers = new List<string>
            {
                $"x-otlp-version={OtlpVersion}",
                $"x-honeycomb-team={options.MetricsApiKey}",
                $"x-honeycomb-dataset={options.MetricsDataset}"
            };
            return string.Join(",", headers);
        }
    }
}