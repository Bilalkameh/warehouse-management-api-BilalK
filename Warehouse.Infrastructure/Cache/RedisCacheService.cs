using Microsoft.Extensions.Caching.Distributed;
using Warehouse.Application.Interfaces;

namespace Warehouse.Infrastructure.Cache;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public Task<string?> GetAsync(string key, CancellationToken cancellationToken)
    {
        return _cache.GetStringAsync(key, cancellationToken);
    }

    public Task SetAsync(string key, string value, TimeSpan expirationTime, CancellationToken cancellationToken)
    {
        var options = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = expirationTime
        };
        
        return _cache.SetStringAsync(key, value, options, cancellationToken);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        return _cache.RemoveAsync(key, cancellationToken);
    }
}