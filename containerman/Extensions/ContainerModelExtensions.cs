using containerman.Models;
using Docker.DotNet.Models;

namespace containerman.Extensions;

internal static class ContainerModelExtensions
{
    public static CreateContainerParameters ToParameters(this CreateContainerRequest request)
    {
        return new CreateContainerParameters()
        {
            Image = request.Image,
        };
    }
}