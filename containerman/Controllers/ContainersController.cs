using System.ComponentModel.DataAnnotations;
using System.Net;
using containerman.Extensions;
using containerman.Models;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Mvc;
using CreateContainerResponse = containerman.Models.CreateContainerResponse;

namespace containerman.Controllers;

[ApiController]
[Route("containers")]
public class ContainersController : Controller
{
    private const string ContainerIdRegex = @"^[\w+-\.]{1,1000}$";

    private readonly ILogger<ContainersController> _logger;
    private readonly IDockerClient _client;

    public ContainersController(ILogger<ContainersController> logger, IDockerClient client)
    {
        _logger = logger;
        _client = client;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateContainerResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Create([FromBody] CreateContainerRequest request,
        CancellationToken cancellationToken = default)
    {
        Docker.DotNet.Models.CreateContainerResponse clientResponse;
        var clientParameters = request.ToParameters();
        try
        {
            clientResponse = await _client.Containers.CreateContainerAsync(clientParameters, cancellationToken);
        }
        catch (DockerImageNotFoundException)
        {
            try
            {
                // TODO make operation asynchronous
                await _client.Images.CreateImageAsync(
                    new ImagesCreateParameters
                    {
                        FromImage = request.Image,
                        Tag = "latest", // TODO
                    },
                    null, // TODO read authConfig from DockerOptions
                    new Progress<JSONMessage>(), cancellationToken);
                clientResponse = await _client.Containers.CreateContainerAsync(clientParameters, cancellationToken);
            }
            catch (DockerImageNotFoundException)
            {
                return NotFound();
            }
            catch (DockerApiException e)
            {
                return StatusCode((int)e.StatusCode);
            }
        }
        catch (DockerApiException e)
        {
            return StatusCode((int)e.StatusCode);
        }


        var response = new CreateContainerResponse
        {
            Id = clientResponse.ID,
        };
        return Ok(response);
    }

    [HttpPost("{id}/start")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Start([FromRoute, RegularExpression(ContainerIdRegex)] string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var clientParameters = new ContainerStartParameters();
            await _client.Containers.StartContainerAsync(id, clientParameters, cancellationToken);
        }
        catch (DockerContainerNotFoundException)
        {
            return NotFound();
        }
        catch (DockerApiException e)
        {
            return StatusCode((int)e.StatusCode);
        }
        return Ok();
    }

    [HttpPost("{id}/stop")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Stop([FromRoute, RegularExpression(ContainerIdRegex)] string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var clientParameters = new ContainerStopParameters();
            await _client.Containers.StopContainerAsync(id, clientParameters, cancellationToken);
        }
        catch (DockerContainerNotFoundException)
        {
            return NotFound();
        }
        catch (DockerApiException e)
        {
            return StatusCode((int)e.StatusCode);
        }
        return Ok();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Delete([FromRoute, RegularExpression(ContainerIdRegex)]string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var clientParameters = new ContainerRemoveParameters();
            await _client.Containers.RemoveContainerAsync(id, clientParameters, cancellationToken);
        }
        catch (DockerContainerNotFoundException)
        {
            return NotFound();
        }
        catch (DockerApiException e)
        {
            return StatusCode((int)e.StatusCode);
        }
        return Ok();
    }
}