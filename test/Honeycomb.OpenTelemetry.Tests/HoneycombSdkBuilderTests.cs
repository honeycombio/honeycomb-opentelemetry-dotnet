using System;
using System.Reflection;
using Xunit;

namespace Honeycomb.OpenTelemetry
{
    public class HoneycombSdkBuilderTests
    {
        [Fact]
        public void BuilderAddsDefaultAttributes()
        {
            var version = typeof(HoneycombSdkBuilder).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            var builder = new HoneycombSdkBuilder();
            var attributes = builder.ResourceBuilder.Build().Attributes;
            Assert.Contains(attributes, item => item.Key == "service.name" && ((string) item.Value).StartsWith("unknown_service"));
            Assert.Contains(attributes, item => item.Key == "honeycomb.distro.language" && (string) item.Value == "dotnet");
            Assert.Contains(attributes, item => item.Key == "honeycomb.distro.version" && (string) item.Value == version);
            Assert.Contains(attributes, item => item.Key == "honeycomb.distro.runtime_version" && (string) item.Value == Environment.Version.ToString());
        }

        [Fact]
        public void CanSetServiceNameAttribute()
        {
            var builder = new HoneycombSdkBuilder()
                .WithServiceName("my-service");

            var attributes = builder.ResourceBuilder.Build().Attributes;
            Assert.Contains(attributes, item => item.Key == "service.name" && (string) item.Value == "my-service");
        }

        [Theory]
        [InlineData("attr-val")]
        [InlineData((long) 1)]
        [InlineData(false)]
        public void CanAddCustomResourceAttributes(object value)
        {
            var builder = new HoneycombSdkBuilder()
                .WithResourceAttribute("custom-attr", value);

            var attributes = builder.ResourceBuilder.Build().Attributes;
            Assert.Contains(attributes, item => item.Key == "custom-attr" && item.Value == value);
        }
    }
}