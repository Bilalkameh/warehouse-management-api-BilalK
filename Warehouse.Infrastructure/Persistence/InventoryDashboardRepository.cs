using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Infrastructure.Persistence;

public class InventoryDashboardRepository : IInventoryDashboardRepository
{
    private readonly IDbContextFactory<WarehouseDbContext> _contextFactory;

    public InventoryDashboardRepository(IDbContextFactory<WarehouseDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<int> GetTotalProductsAsync(CancellationToken cancellationToken)
    {
        using var context = _contextFactory.CreateDbContext();

        return await context.Products
            .CountAsync(cancellationToken);
    }

    public async Task<int> GetAvailableProductsAsync(CancellationToken cancellationToken)
    {
        using var context = _contextFactory.CreateDbContext();

        return await context.Products
            .CountAsync(product =>
                    product.QuantityInStock > 0 && !product.IsArchived,
                cancellationToken);
    }

    public async Task<int> GetActiveSuppliersAsync(
        CancellationToken cancellationToken)
    {
        using var context = _contextFactory.CreateDbContext();

        return await context.Suppliers
            .CountAsync(supplier => supplier.IsActive, cancellationToken);
    }
}