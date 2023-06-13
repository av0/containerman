using Docker.DotNet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit.Abstractions;

namespace containerman.Tests;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly Mock<IDockerClient> _dockerClientMock = new();
    public Mock<IContainerOperations> ContainerOperationsMock { get; } = new();
    public Mock<IImageOperations> ImageOperationsMock { get; } = new();

    public ITestOutputHelper Output { get; set; } = null!;

    public CustomWebApplicationFactory()
    {
        _dockerClientMock.SetupGet(x => x.Containers)
            .Returns(ContainerOperationsMock.Object);
        _dockerClientMock.SetupGet(x => x.Images)
            .Returns(ImageOperationsMock.Object);
    }


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddScoped<IDockerClient>(_ => _dockerClientMock.Object);
        });

        builder.UseEnvironment("Development");
    }

}
