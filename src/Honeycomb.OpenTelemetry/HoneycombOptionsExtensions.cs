using System;
using System.Collections.Generic;

namespace Honeycomb.OpenTelemetry {
    internal static class HoneycombOptionsExtensions {
        private const string OtlpVersion = "0.16.0";

        internal static string GetTraceHeaders(this HoneycombOptions options) {
            return GetHeaders(
                options.IsLegacyKey(options.TracesApiKey),
                options.TracesApiKey,
                options.TracesDataset
            );
        }

        internal static string GetMetricsHeaders(this HoneycombOptions options) {
            return GetHeaders(
                options.IsLegacyKey(options.MetricsApiKey),
                options.MetricsApiKey,
                options.MetricsDataset
            );
        }

        private static string GetHeaders(bool isLegacyKey, string apikey, string dataset) {
            var headers = new List<string>
            {
                $"x-otlp-version={OtlpVersion}",
                $"x-honeycomb-team={apikey}"
            };
            if (isLegacyKey) {
                // if the key is legacy, add dataset to the header
                if (!string.IsNullOrWhiteSpace(dataset)) {
                    headers.Add($"x-honeycomb-dataset={dataset}");
                } else {
                    // if legacy key and missing dataset, warn on missing dataset
                    Console.WriteLine($"WARN: {EnvironmentOptions.GetErrorMessage("dataset", "HONEYCOMB_DATASET")}.");
                }
            }

            return string.Join(",", headers);
        }
    }
}