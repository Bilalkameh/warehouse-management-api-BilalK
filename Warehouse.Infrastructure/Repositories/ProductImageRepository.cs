using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;
using Warehouse.Infrastructure.Models;

namespace Warehouse.Infrastructure.Repositories;

public class ProductImageRepository : IProductImageRepository
{
    private readonly WarehouseDbFirstContext _context;

    public ProductImageRepository(WarehouseDbFirstContext context)
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