using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Queries.Shipments.GetShipmentById;

public class GetShipmentByIdRequest : IRequest<ShipmentViewModel>
{
    public Guid ShipmentId { get; set; }
}