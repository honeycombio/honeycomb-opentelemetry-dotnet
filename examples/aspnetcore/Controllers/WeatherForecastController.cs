using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using System.Diagnostics.Metrics;

namespace aspnetcore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly Tracer _tracer;
        private readonly Counter<int> _counter;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, Tracer tracer, Meter meter)
        {
            _logger = logger;
            _tracer = tracer;
            _counter = meter.CreateCounter<int>("sheep");
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            using (var span = _tracer.StartActiveSpan("sleep"))
            {
                span.SetAttribute("delay_ms", 100);
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }

            _counter.Add(1);
            
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
