using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Infrastructure.Repositories;

public class FakeProductImageRepository : IProductImageRepository
{
    private readonly List<ProductImage> _images = new();
    
    public void Add(ProductImage image)
    {
        _images.Add(image);
    }
    
    public List<ProductImage> GetByProductId(Guid productId)
    {
        return _images
            .Where(i => i.ProductId == productId)
            .ToList();
    }
}