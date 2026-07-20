using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Entities;

namespace Warehouse.Infrastructure.Persistence;

public class WarehouseDbContext : DbContext
{
    public WarehouseDbContext(
        DbContextOptions<WarehouseDbContext> options) : base(options)
    {
        
    }

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Supplier> Suppliers => Set<Supplier>();

    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
}