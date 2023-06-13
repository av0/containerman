using System.Text;
using System.Text.Json;

namespace containerman.Tests;

internal static class JsonExtensions
{
    public static StringContent ToJsonContent(this object obj)
    {
        var jsonContent = JsonSerializer.Serialize(obj);
        return new StringContent(jsonContent, Encoding.UTF8, "application/json");
    }
}