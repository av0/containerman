using System.Net;
using System.Text.Json;
using containerman.Models;
using Docker.DotNet;
using Docker.DotNet.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Xunit.Abstractions;
using CreateContainerResponse = containerman.Models.CreateContainerResponse;

namespace containerman.Tests;

public partial class ContainerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ContainerTests(ITestOutputHelper output, CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions());
        _factory.ContainerOperationsMock.Reset();
    }
}