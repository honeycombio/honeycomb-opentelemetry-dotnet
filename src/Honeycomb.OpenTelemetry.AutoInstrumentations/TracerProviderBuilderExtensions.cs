using Npgsql;

namespace OpenTelemetry.Trace
{
    /// <summary>
    /// Extension methods to add instrumentation support for many common instrumentation packages.
    /// </summary>
    public static class TracerProviderBuilderExtensions
    {
        /// <summary>
        /// Adds support for common instrumentation packages. Refer to documentation for a complete list of included instrumentations.
        /// </summary>
        /// <param name="builder"><see cref="TracerProviderBuilder"/> being configured.</param>
        /// <returns>The instance of <see cref="TracerProviderBuilder"/> to chain the calls.</returns>
        public static TracerProviderBuilder AddAutoInstrumentations(this TracerProviderBuilder builder)
        {
            return
                builder
#if NET6_0_OR_GREATER
                .AddAspNetCoreInstrumentationWithBaggage()
                .AddGrpcClientInstrumentation()
#elif NET462_OR_GREATER
                .AddAspNetInstrumentation()
#endif
#if NETSTANDARD2_0_OR_GREATER
                .AddQuartzInstrumentation()
#endif
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddMySqlDataInstrumentation()
                .AddRedisInstrumentation()
                .AddWcfInstrumentation()
                .AddNpgsql();
        }
    }
}