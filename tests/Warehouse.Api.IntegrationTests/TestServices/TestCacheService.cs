using Warehouse.Application.Interfaces;

namespace Warehouse.Api.IntegrationTests.TestServices;

public class TestCacheService : ICacheService
{
    public Task<string?> GetAsync(string key, CancellationToken cancellationToken)
    {
        return Task.FromResult<string?>(null);
    }

    public Task SetAsync(string key, string value, TimeSpan expirationTime, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}