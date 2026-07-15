using MediatR;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Inventory.GetInventoryDashboard;

public class GetInventoryDashboardHandler : IRequestHandler<GetInventoryDashboardRequest, InventoryDashboardViewModel>
{
    private readonly IInventoryDashboardRepository _repository;

    public GetInventoryDashboardHandler(IInventoryDashboardRepository repository)
    {
        _repository = repository;
    }

    public async Task<InventoryDashboardViewModel> Handle(GetInventoryDashboardRequest request, CancellationToken cancellationToken)
    {
        var totalProductsTask = _repository.GetTotalProductsAsync(cancellationToken);

        var availableProductsTask = _repository.GetAvailableProductsAsync(cancellationToken);

        var activeSuppliersTask = _repository.GetActiveSuppliersAsync(cancellationToken);

        await Task.WhenAll(totalProductsTask, availableProductsTask, activeSuppliersTask);

        return new InventoryDashboardViewModel
        {
            TotalProducts = await totalProductsTask,
            AvailableProducts = await availableProductsTask,
            ActiveSuppliers = await activeSuppliersTask
        };
    }
}