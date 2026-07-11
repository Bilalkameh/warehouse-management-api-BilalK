using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Infrastructure.Persistence;

public class ProductImageRepository : IProductImageRepository
{
    private readonly WarehouseDbContext _context;

    public ProductImageRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public void Add(ProductImage image)
    {
        _context.ProductImages.Add(image);
        _context.SaveChanges();
    }

    public List<ProductImage> GetByProductId(Guid productId)
    {
        return _context.ProductImages
            .Where(image => image.ProductId == productId)
            .ToList();
    }
}