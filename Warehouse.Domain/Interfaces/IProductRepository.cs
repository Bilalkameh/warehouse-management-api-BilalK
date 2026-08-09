using Warehouse.Domain.Entities;

namespace Warehouse.Domain.Interfaces;

public interface IProductRepository
{
    Task<List <Product>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<Product>> GetAvailableAsync(CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Product  product, CancellationToken cancellationToken);
    Task UpdateAsync(Product  product, CancellationToken cancellationToken);
	Task<List<Product>> SearchAsync(string? name, string? supplier, CancellationToken cancellationToken);
	Task AdjustStockAsync(Product product, StockMovement movement, CancellationToken cancellationToken);
	Task<List<Product>> GetExpiringProductsAsync(DateTime date, CancellationToken cancellationToken);
	Task<List<Product>> GetExpiringSoonAsync(DateTime startDate, DateTime endDateExclusive, CancellationToken cancellationToken);
	Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken);
	Task<List<Product>> GetOutOfStockAsync(CancellationToken cancellationToken);
}