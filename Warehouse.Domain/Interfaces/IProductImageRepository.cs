using Warehouse.Domain.Entities;

namespace Warehouse.Domain.Interfaces;

public interface IProductImageRepository
{
    void Add(ProductImage image);

    List<ProductImage> GetByProductId(Guid productId);
}