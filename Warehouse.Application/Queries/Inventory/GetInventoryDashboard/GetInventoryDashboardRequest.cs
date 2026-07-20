using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Queries.Inventory.GetInventoryDashboard;

public class GetInventoryDashboardRequest : IRequest<InventoryDashboardViewModel>
{
}