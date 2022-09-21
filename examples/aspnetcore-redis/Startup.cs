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
using OpenTelemetry;
using OpenTelemetry.Trace;
using Honeycomb.OpenTelemetry;
using StackExchange.Redis;

namespace aspnetcoreredis
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

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
                new ConfigurationOptions
                {
                    EndPoints = { "localhost:6379" },
                    AbortOnConnectFail = false, // allow for reconnects if redis is not available
                }
            );
            services.AddSingleton<IConnectionMultiplexer>(redis);

            // configure OpenTelemetry SDK to send data to Honeycomb
            var options = Configuration.GetSection(HoneycombOptions.ConfigSectionName).Get<HoneycombOptions>();
            services.AddOpenTelemetryTracing(builder => builder
                .AddHoneycomb(options)
                .AddAspNetCoreInstrumentationWithBaggage()
                .AddRedisInstrumentation(redis)
            );
            services.AddSingleton(TracerProvider.Default.GetTracer(options.ServiceName));
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