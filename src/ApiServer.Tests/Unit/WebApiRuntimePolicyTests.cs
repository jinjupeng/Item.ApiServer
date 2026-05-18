using ApiServer.WebApi.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace ApiServer.Tests.Unit;

public class WebApiRuntimePolicyTests
{
    [Fact]
    public void ShouldRequireHttpsMetadata_ShouldDefaultToTrue_InProduction()
    {
        var environment = CreateEnvironment("Production");
        var configuration = new ConfigurationBuilder().Build();

        var result = WebApiRuntimePolicy.ShouldRequireHttpsMetadata(environment.Object, configuration);

        result.Should().BeTrue();
    }

    [Fact]
    public void ShouldRequireHttpsMetadata_ShouldDefaultToFalse_InDevelopment()
    {
        var environment = CreateEnvironment("Development");
        var configuration = new ConfigurationBuilder().Build();

        var result = WebApiRuntimePolicy.ShouldRequireHttpsMetadata(environment.Object, configuration);

        result.Should().BeFalse();
    }

    [Fact]
    public void ShouldRunAutoInitialization_ShouldDefaultToFalse_InProduction()
    {
        var environment = CreateEnvironment("Production");
        var configuration = new ConfigurationBuilder().Build();

        var result = WebApiRuntimePolicy.ShouldRunAutoInitialization(environment.Object, configuration);

        result.Should().BeFalse();
    }

    [Fact]
    public void GetAllowedCorsOrigins_ShouldTrimAndSplitConfiguredOrigins()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Cors:AllowedOrigins"] = " https://a.example.com ,https://b.example.com "
            })
            .Build();

        var origins = WebApiRuntimePolicy.GetAllowedCorsOrigins(configuration);

        origins.Should().Equal("https://a.example.com", "https://b.example.com");
    }

    private static Mock<IHostEnvironment> CreateEnvironment(string environmentName)
    {
        var environment = new Mock<IHostEnvironment>();
        environment.SetupGet(x => x.EnvironmentName).Returns(environmentName);
        return environment;
    }
}
