using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Honeycomb.OpenTelemetry;
using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace aspnetcore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // configure OpenTelemetry SDK to send data to Honeycomb
            var options = Configuration.GetSection(HoneycombOptions.ConfigSectionName).Get<HoneycombOptions>();
            services.AddOpenTelemetryTracing(builder => builder
                .AddHoneycomb(options)
                .AddAspNetCoreInstrumentationWithBaggage()
            );
            services.AddSingleton(TracerProvider.Default.GetTracer(options.ServiceName));

            // (optional metrics setup)
            // meter name used here must be configured in the OpenTelemetry SDK
            // service name is configured by default
            // you may configure additional meter names using the Honeycomb options
            Meter meter = new Meter(options.MetricsDataset);
            services.AddSingleton(meter);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}