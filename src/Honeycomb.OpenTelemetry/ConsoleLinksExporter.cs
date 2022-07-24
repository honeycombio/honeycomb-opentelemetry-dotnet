using OpenTelemetry;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Honeycomb.OpenTelemetry
{
    /// <summary>
    /// Writes links to the Honeycomb for all root spans
    /// </summary>
    public class ConsoleLinkExporter : BaseExporter<Activity>
    {
        private readonly HoneycombOptions _options;
        private string _teamSlug;
        private string _environmentSlug;

        private bool IsEnabled = false;

        /// <summary>
        /// Initializes the <see cref="ConsoleLinkExporter" /> class
        /// </summary>
        /// <param name="options">Settings for Link generation</param>
        public ConsoleLinkExporter(HoneycombOptions options)
        {
            _options = options;
            InitTraceLinkParameters();
        }

        private void InitTraceLinkParameters()
        {
            if (string.IsNullOrEmpty(_options.ApiKey))
                return;

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://api.honeycomb.io/1/auth");
            httpClient.DefaultRequestHeaders.Add("X-Honeycomb-Team", _options.ApiKey);

            var response = httpClient.GetAsync("").GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) {
                Console.WriteLine("Didn't get a valid response from Honeycomb");
                return;
            }

            var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseString);
            _environmentSlug = authResponse.Environment.Slug;
            _teamSlug = authResponse.Team.Slug;
            if (string.IsNullOrEmpty(_environmentSlug) ||
                string.IsNullOrEmpty(_teamSlug)) {
                Console.WriteLine("Team or Environment wasn't returned");
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
                    Console.WriteLine($"Trace Emitted for {activity.DisplayName}");
                    Console.WriteLine($"TraceLink: {GetTraceLink(activity.TraceId.ToString())}");
                }
            }
            return ExportResult.Success;
        }

        private string GetTraceLink(string traceId)
        {
            var dataset = _options.ServiceName;   
            return $"http://ui.honeycomb.io/{_teamSlug}/environments/{_environmentSlug}/datasets/{dataset}/trace?trace_id={traceId}";
        }
    }

    #pragma warning disable 1591
    public class AuthResponse
    {
        [JsonPropertyName("environment")]
        public HoneycombEnvironment Environment { get; set; }

        [JsonPropertyName("team")]
        public Team Team { get; set; }
    }
    public class HoneycombEnvironment
    {
        [JsonPropertyName("slug")]
        public string Slug { get; set; }
    }

    public class Team
    {
        [JsonPropertyName("slug")]
        public string Slug { get; set; }
    }

}
