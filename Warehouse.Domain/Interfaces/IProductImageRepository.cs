using Warehouse.Domain.Entities;

namespace Warehouse.Domain.Interfaces;

public interface IProductImageRepository
{
    Task AddAsync(ProductImage image, CancellationToken cancellationToken);

    Task<List<ProductImage>> GetByProductIdAsync (Guid productId, CancellationToken cancellationToken);
    
    Task<ProductImage?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task DeleteAsync(ProductImage image, CancellationToken cancellationToken);

    Task ReplaceAsync(ProductImage oldImage, ProductImage newImage, CancellationToken cancellationToken);
}