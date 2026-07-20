namespace Warehouse.Domain.Interfaces;

public interface IInventoryDashboardRepository
{
    Task<int> GetTotalProductsAsync(CancellationToken cancellationToken);

    Task<int> GetAvailableProductsAsync(CancellationToken cancellationToken);

    Task<int> GetActiveSuppliersAsync(CancellationToken cancellationToken);
}