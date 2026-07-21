using Microsoft.Extensions.Logging;
using Warehouse.Application.Cache;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.BackgroundJobs;

public class ProductExpiryJob
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductExpiryJob> _logger;
    private readonly ICacheService _cache;

    public ProductExpiryJob(IProductRepository productRepository, ILogger<ProductExpiryJob> logger, ICacheService cache)
    {
        _productRepository = productRepository;
        _logger = logger;
        _cache = cache;
    }

    public async Task CheckProductExpiryAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var nextThirtyDays = today.AddDays(30);

        // Filter the products by their expiry dates
        var products = await _productRepository.GetExpiringProductsAsync(nextThirtyDays, cancellationToken);

        var expiredProducts = products
            .Where(product => product.ExpiryDate.Date < today)
            .ToList();

        var expiringProducts = products
            .Where(product => product.ExpiryDate.Date >= today && product.ExpiryDate.Date <= nextThirtyDays)
            .ToList();
        
        var expiredNames = expiredProducts.Count > 0
            ? string.Join(", ", expiredProducts.Select(product => product.Name))
            : "None";

        var expiringNames = expiringProducts.Count > 0
            ? string.Join(", ", expiringProducts.Select(product => product.Name))
            : "None";

        _logger.LogInformation("Product expiry check completed. Expired products: {ExpiredCount}. Products expiring within 30 days: {ExpiringSoonCount}", expiredProducts.Count, expiringProducts.Count);

        _logger.LogInformation("Expired product names: {ExpiredProductNames}", expiredNames);
        
        _logger.LogInformation("Products expiring within 30 days: {ExpiringSoonProductNames}", expiringNames);

        
        // Archive products that have been expired for more than 7 days
        var sevenDaysAgo = today.AddDays(-7);

        var productsToArchive = expiredProducts
            .Where(product => product.ExpiryDate.Date < sevenDaysAgo && !product.IsArchived)
            .ToList();

        foreach (var product in productsToArchive)
        {
            product.Archive();

            await _productRepository.UpdateAsync(product, cancellationToken);

            await _cache.RemoveAsync(ProductCacheKeys.ById(product.Id), cancellationToken);

            _logger.LogInformation("Archived expired product {ProductName} with ID {ProductId}", product.Name, product.Id);
        }
    
        // Remove the cached product because its changed
        if (productsToArchive.Count > 0)
        {
            await _cache.RemoveAsync(ProductCacheKeys.All, cancellationToken);

            await _cache.RemoveAsync(ProductCacheKeys.Available, cancellationToken);
        }

        _logger.LogInformation("Archived {ArchivedCount} products expired for more than seven days", productsToArchive.Count);
    }
}