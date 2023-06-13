using System.ComponentModel.DataAnnotations;

namespace containerman.Models;

public class CreateContainerRequest
{
    [Required] public string Image { get; set; } = null!;
}