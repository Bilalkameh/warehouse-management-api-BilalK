using Microsoft.Extensions.Logging;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.BackgroundJobs;

public class ProductExpiryJob
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger <ProductExpiryJob> _logger;


    public ProductExpiryJob(IProductRepository productRepository, ILogger<ProductExpiryJob> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task CheckProductExpiryAsync()
    {
        var products = await _productRepository.GetAllAsync(CancellationToken.None);
        var today = DateTime.UtcNow.Date;
        var nextThirtyDays = today.AddDays(30);

        var expiredProducts = products.Where(product => product.ExpiryDate < today)
            .ToList();
        
        var expiringProducts = products.Where(product => product.ExpiryDate <= nextThirtyDays && product.ExpiryDate >= today)
            .ToList();
        
        var expiredNames = expiredProducts.Count > 0
            ? string.Join(", ", expiredProducts.Select(product => product.Name))
            : "None";
        
        var expiringNames = expiringProducts.Count > 0
            ? string.Join(", ", expiringProducts.Select(product => product.Name))
            : "None";
        
        _logger.LogInformation("Product expiry check completed. Expired products: {ExpiredCount}. Products expiring within 30 days: {ExpiringSoonCount}",
            expiredProducts.Count, expiringProducts.Count);
        
        _logger.LogInformation("Expired product names: {ExpiredProductNames}", expiredNames);
        
        _logger.LogInformation("Products expiring within 30 days: {ExpiringSoonProductNames}", expiringNames);

    }
    


}