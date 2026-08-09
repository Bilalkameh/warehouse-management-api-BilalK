using System.Text;
using System.Text.Json;

namespace Warehouse.Api.UnitTests.Helpers;

public static class JsonContentHelper
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
    
    public static StringContent Create<T>(T value)
    
    {
        var json = JsonSerializer.Serialize(value, Options);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}