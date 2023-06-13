using System.Net;
using System.Text.Json;
using containerman.Models;
using Docker.DotNet;
using Docker.DotNet.Models;
using FluentAssertions;
using Moq;
using CreateContainerResponse = containerman.Models.CreateContainerResponse;

namespace containerman.Tests;

public partial class ContainerTests
{
    [Fact]
    public async void Create_HappyPath_Success()
    {
        // Arrange
        var expectedId = "abcd1234";
        _factory.ContainerOperationsMock.Setup(x => x.CreateContainerAsync(It.IsAny<CreateContainerParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Docker.DotNet.Models.CreateContainerResponse()
            {
                ID = expectedId,
            });

        var request = new CreateContainerRequest
        {
            Image = "nginx",
        };

        // Act
        var response = await _httpClient.PostAsync("containers", request.ToJsonContent());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseObject =
            JsonSerializer.Deserialize<CreateContainerResponse>(await response.Content.ReadAsStringAsync());
        responseObject!.Id.Should().Be(expectedId);
        _factory.ContainerOperationsMock.Verify(x => x.CreateContainerAsync(
            It.Is<CreateContainerParameters>(x => true),
            It.IsAny<CancellationToken>()
        ));
        _factory.ContainerOperationsMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async void Create_NoImage_BadRequest()
    {
        // Arrange
        var expectedId = "abcd1234";
        _factory.ContainerOperationsMock.Setup(x => x.CreateContainerAsync(It.IsAny<CreateContainerParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Docker.DotNet.Models.CreateContainerResponse()
            {
                ID = expectedId,
            });

        var request = new CreateContainerRequest
        {
            Image = null!,
        };

        // Act
        var response = await _httpClient.PostAsync("containers", request.ToJsonContent());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _factory.ContainerOperationsMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async void Create_UnknownImage_NotFound()
    {
        // Arrange
        _factory.ContainerOperationsMock.Setup(x => x.CreateContainerAsync(It.IsAny<CreateContainerParameters>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DockerImageNotFoundException(HttpStatusCode.NotFound, string.Empty));
        _factory.ImageOperationsMock.Setup(x => x.CreateImageAsync(
                It.IsAny<ImagesCreateParameters>(),
                It.IsAny<AuthConfig>(),
                It.IsAny<IProgress<JSONMessage>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DockerApiException(HttpStatusCode.NotFound, string.Empty));

        var request = new CreateContainerRequest
        {
            Image = "foo",
        };

        // Act
        var response = await _httpClient.PostAsync("containers", request.ToJsonContent());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _factory.ContainerOperationsMock.Verify(x => x.CreateContainerAsync(
            It.Is<CreateContainerParameters>(x => true),
            It.IsAny<CancellationToken>()
        ));
        _factory.ImageOperationsMock.Verify(x=> x.CreateImageAsync(
            It.Is<ImagesCreateParameters>(x => x.FromImage == "foo"), 
            null,
            It.IsAny<IProgress<JSONMessage>>(),
            It.IsAny<CancellationToken>()));
        _factory.ContainerOperationsMock.VerifyNoOtherCalls();
        _factory.ImageOperationsMock.VerifyNoOtherCalls();
    }
    
}