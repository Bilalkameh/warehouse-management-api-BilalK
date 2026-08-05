using Warehouse.Application.Interfaces;

namespace Warehouse.Application.Cache;

internal static class ProductCacheInvalidator
{
    public static async Task InvalidateProductAsync(ICacheService cache, Guid productId, CancellationToken cancellationToken)
    {
        await cache.RemoveAsync(ProductCacheKeys.ById(productId), cancellationToken);

        await InvalidateProductListsAsync(cache, cancellationToken);
    }

    public static async Task InvalidateProductListsAsync(ICacheService cache, CancellationToken cancellationToken)
    {
        await cache.RemoveAsync(ProductCacheKeys.All, cancellationToken);

        await cache.RemoveAsync(ProductCacheKeys.Available, cancellationToken);
    }
}