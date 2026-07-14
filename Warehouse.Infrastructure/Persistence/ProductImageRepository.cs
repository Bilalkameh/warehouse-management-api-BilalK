using Microsoft.EntityFrameworkCore;
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

    public async Task AddAsync(ProductImage image, CancellationToken cancellationToken)
    {
        await _context.ProductImages.AddAsync(
            image,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<List<ProductImage>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await _context.ProductImages
            .Where(image => image.ProductId == productId)
            .ToListAsync(cancellationToken);
    }
}