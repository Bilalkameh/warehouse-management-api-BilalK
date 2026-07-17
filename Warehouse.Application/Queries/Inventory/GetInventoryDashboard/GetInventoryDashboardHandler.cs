using MediatR;
using Microsoft.Extensions.Logging;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Inventory.GetInventoryDashboard;

public class GetInventoryDashboardHandler : IRequestHandler<GetInventoryDashboardRequest, InventoryDashboardViewModel>
{
    private readonly IInventoryDashboardRepository _repository;
    private readonly ILogger<GetInventoryDashboardHandler> _logger;

    public GetInventoryDashboardHandler(IInventoryDashboardRepository repository, ILogger<GetInventoryDashboardHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<InventoryDashboardViewModel> Handle(GetInventoryDashboardRequest request, CancellationToken cancellationToken)
    {
        int? totalProducts =null;
        int? availableProducts =null;
        int? activeSuppliers = null;

        try
        {
            totalProducts =await _repository.GetTotalProductsAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception,"Failed to load total products dashboard graph.");
        }

        try
        {
            availableProducts = await _repository.GetAvailableProductsAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception,"Failed to load available products dashboard graph.");
        }

        try
        {
            activeSuppliers = await _repository.GetActiveSuppliersAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception,"Failed to load active suppliers dashboard graph.");
        }

        return new InventoryDashboardViewModel
        {
            TotalProducts = totalProducts,
            AvailableProducts = availableProducts,
            ActiveSuppliers = activeSuppliers
        };
    }
}