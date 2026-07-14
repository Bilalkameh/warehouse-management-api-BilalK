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
}