using System.Net;
using Docker.DotNet;
using Docker.DotNet.Models;
using FluentAssertions;
using Moq;

namespace containerman.Tests;

public partial class ContainerTests
{
    [Fact]
    public async void Start_HappyPath_Success()
    {
        // Arrange
        var id = "abcd1234";

        // Act
        var response = await _httpClient.PostAsync($"containers/{id}/start", new StringContent(string.Empty));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.ContainerOperationsMock.Verify(x => x.StartContainerAsync(
            id,
            It.Is<ContainerStartParameters>(x => true),
            It.IsAny<CancellationToken>()
        ));
        _factory.ContainerOperationsMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async void Start_NonExistingId_NotFound()
    {
        // Arrange
        var id = "abcd1234";
        _factory.ContainerOperationsMock.Setup(x => x.StartContainerAsync(id, It.IsAny<ContainerStartParameters>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DockerContainerNotFoundException(HttpStatusCode.NotFound, string.Empty));

        // Act
        var response = await _httpClient.PostAsync($"containers/{id}/start", new StringContent(string.Empty));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _factory.ContainerOperationsMock.Verify(x => x.StartContainerAsync(
            id,
            It.Is<ContainerStartParameters>(x => true),
            It.IsAny<CancellationToken>()
        ));
        _factory.ContainerOperationsMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async void Start_NotCorrectId_BadRequest()
    {
        // Arrange
        var id = "!2323";
        _factory.ContainerOperationsMock.Setup(x => x.StartContainerAsync(id, It.IsAny<ContainerStartParameters>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DockerContainerNotFoundException(HttpStatusCode.NotFound, string.Empty));

        // Act
        var response = await _httpClient.PostAsync($"containers/{id}/start", new StringContent(string.Empty));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        _factory.ContainerOperationsMock.VerifyNoOtherCalls();
    }
}