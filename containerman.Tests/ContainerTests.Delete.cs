using System.Net;
using Docker.DotNet;
using Docker.DotNet.Models;
using FluentAssertions;
using Moq;

namespace containerman.Tests;

public partial class ContainerTests
{
    [Fact]
    public async void Delete_HappyPath_Success()
    {
        // Arrange
        var id = "abcd1234";

        // Act
        var response = await _httpClient.DeleteAsync($"containers/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.ContainerOperationsMock.Verify(x => x.RemoveContainerAsync(
            id,
            It.Is<ContainerRemoveParameters>(x => true),
            It.IsAny<CancellationToken>()
        ));
        _factory.ContainerOperationsMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async void Delete_NonExistingId_NotFound()
    {
        // Arrange
        var id = "abcd1234";
        _factory.ContainerOperationsMock.Setup(x => x.RemoveContainerAsync(id, It.IsAny<ContainerRemoveParameters>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DockerContainerNotFoundException(HttpStatusCode.NotFound, string.Empty));

        // Act
        var response = await _httpClient.DeleteAsync($"containers/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _factory.ContainerOperationsMock.Verify(x => x.RemoveContainerAsync(
            id,
            It.Is<ContainerRemoveParameters>(x => true),
            It.IsAny<CancellationToken>()
        ));
        _factory.ContainerOperationsMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async void Delete_NotCorrectId_BadRequest()
    {
        // Arrange
        var id = "!2323";
        _factory.ContainerOperationsMock.Setup(x => x.RemoveContainerAsync(id, It.IsAny<ContainerRemoveParameters>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DockerContainerNotFoundException(HttpStatusCode.NotFound, string.Empty));

        // Act
        var response = await _httpClient.DeleteAsync($"containers/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        _factory.ContainerOperationsMock.VerifyNoOtherCalls();
    }
}