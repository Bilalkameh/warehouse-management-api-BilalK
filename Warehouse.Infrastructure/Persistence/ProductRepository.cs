using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Infrastructure.Persistence;

public class ProductRepository : IProductRepository
{
    private readonly WarehouseDbContext _context;

    public ProductRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Products
            .Include(product => product.Supplier)
            .ToListAsync(cancellationToken);
    }   

    public async Task <Product?> GetByIdAsync(Guid id,CancellationToken cancellationToken)
    {
        return await _context.Products
            .Include(product => product.Supplier)
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public async Task<List<Product>> GetAvailableAsync(CancellationToken cancellationToken)
    {
        return await _context.Products
            .Include(product => product.Supplier)
            .Where(product => product.QuantityInStock > 0 && !product.IsArchived)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await _context.Products.AddAsync(product,cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Product>> SearchAsync(string? name, string? supplierName, CancellationToken cancellationToken)
    {
        var query = _context.Products
            .Include(product => product.Supplier)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(product =>
                product.Name.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(supplierName))
        {
            query = query.Where(product =>
                product.Supplier.Name.Contains(supplierName));
        }

        return await query.ToListAsync(cancellationToken);
    }
    
    //Stock adjustment endpoint
    public async Task AdjustStockAsync(Product product, StockMovement movement, CancellationToken cancellationToken)
    {
        _context.Products.Update(product);

        await _context.StockMovements.AddAsync(movement, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<List<Product>> GetExpiringProductsAsync(DateTime date, CancellationToken cancellationToken)
    {
        return await _context.Products
            .Where(product => product.ExpiryDate <= date)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        return await _context.Products.AnyAsync(product => product.SKU == sku, cancellationToken);
    }
}