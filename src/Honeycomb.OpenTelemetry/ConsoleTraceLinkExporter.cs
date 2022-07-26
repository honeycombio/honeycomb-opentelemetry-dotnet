using OpenTelemetry;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Honeycomb.OpenTelemetry
{
    /// <summary>
    /// Writes links to the Honeycomb UI for all root spans to the console
    /// </summary>
    internal class ConsoleTraceLinkExporter : BaseExporter<Activity>
    {
        private const string ApiHost = "https://api.honeycomb.io";
        private string _apiKey;
        private string _serviceName;
        private string _teamSlug;
        private string _environmentSlug;

        private bool IsEnabled = false;

        /// <summary>
        /// Initializes the <see cref="ConsoleTraceLinkExporter" /> class
        /// </summary>
        /// <param name="options">Settings for Link generation</param>
        public ConsoleTraceLinkExporter(HoneycombOptions options)
        {
            _apiKey = options.ApiKey;
            _serviceName = options.ServiceName;
            try {
                InitTraceLinkParameters();
            }
            catch (Exception)
            {
                Console.WriteLine("WARN: Failed to get data from Honeycomb Auth Endpoint");
            }
        }

        private void InitTraceLinkParameters()
        {
            if (string.IsNullOrEmpty(_apiKey))
                return;

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ApiHost);
            httpClient.DefaultRequestHeaders.Add("X-Honeycomb-Team", _apiKey);

            var response = httpClient.GetAsync("/1/auth").GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) {
                Console.WriteLine("WARN: Didn't get a valid response from Honeycomb");
                return;
            }

            var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseString);
            _environmentSlug = authResponse.Environment.Slug;
            _teamSlug = authResponse.Team.Slug;
            if (string.IsNullOrEmpty(_environmentSlug) ||
                string.IsNullOrEmpty(_teamSlug)) {
                Console.WriteLine("WARN: Team or Environment wasn't returned");
                return;                
            }
            IsEnabled = true;
        }

        /// <inheritdoc />
        public override ExportResult Export(in Batch<Activity> batch)
        {
            if (!IsEnabled)
                return ExportResult.Success;

            foreach (var activity in batch)
            {
                if (string.IsNullOrEmpty(activity.ParentId))
                {
                    Console.WriteLine($"Trace for {activity.DisplayName}" + Environment.NewLine + 
                                      $"Honeycomb link: {GetTraceLink(activity.TraceId.ToString())}");
                }
            }
            return ExportResult.Success;
        }

        private string GetTraceLink(string traceId)
        {
            if (_apiKey.Length == 32)
               return $"http://ui.honeycomb.io/{_teamSlug}/datasets/{_serviceName}/trace?trace_id={traceId}";

            return $"http://ui.honeycomb.io/{_teamSlug}/environments/{_environmentSlug}/datasets/{_serviceName}/trace?trace_id={traceId}";
        }
    }

    internal class AuthResponse
    {
        [JsonPropertyName("environment")]
        public HoneycombEnvironment Environment { get; set; }

        [JsonPropertyName("team")]
        public Team Team { get; set; }
    }
    internal class HoneycombEnvironment
    {
        [JsonPropertyName("slug")]
        public string Slug { get; set; }
    }

    internal class Team
    {
        [JsonPropertyName("slug")]
        public string Slug { get; set; }
    }

}
