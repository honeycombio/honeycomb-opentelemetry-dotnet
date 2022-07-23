using OpenTelemetry;
using System;
using System.Diagnostics;

namespace Honeycomb.OpenTelemetry
{
    /// <summary>
    /// 
    /// </summary>
    public class ConsoleLinkExporter : BaseExporter<Activity>
    {
        private readonly HoneycombOptions _options;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public ConsoleLinkExporter(HoneycombOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        public override ExportResult Export(in Batch<Activity> batch)
        {
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
            var team = _options.Team;
            var environment = _options.Environment;
            var dataset = _options.ServiceName;   
            return $"http://ui.honeycomb.io/{team}/environments/{environment}/datasets/{dataset}/trace?trace_id={traceId}";
        }
    }

}
