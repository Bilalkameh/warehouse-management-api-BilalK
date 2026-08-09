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
    
    public DbSet<SupplierDocument> SupplierDocuments => Set<SupplierDocument>();
    
    public DbSet<Shipment> Shipments => Set<Shipment>();

    public DbSet<ShipmentProduct> ShipmentProducts =>
        Set<ShipmentProduct>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Shipment>()
            .HasIndex(shipment => shipment.TrackingNumber)
            .IsUnique();

        modelBuilder.Entity<Shipment>()
            .Property(shipment => shipment.Status)
            .HasConversion<string>();

        modelBuilder.Entity<ShipmentProduct>()
            .HasIndex(item => new
            {
                item.ShipmentId,
                item.ProductId
            })
            .IsUnique();
    }
}