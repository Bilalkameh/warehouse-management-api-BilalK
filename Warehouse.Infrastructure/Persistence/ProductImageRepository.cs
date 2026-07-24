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
    
    public async Task<ProductImage?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.ProductImages
            .FirstOrDefaultAsync(image => image.Id == id, cancellationToken);
    }

    public async Task DeleteAsync(ProductImage image, CancellationToken cancellationToken)
    {
        _context.ProductImages.Remove(image);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceAsync(ProductImage oldImage, ProductImage newImage, CancellationToken cancellationToken)
    {
        _context.ProductImages.Remove(oldImage);

        await _context.ProductImages.AddAsync(newImage, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
    
    
