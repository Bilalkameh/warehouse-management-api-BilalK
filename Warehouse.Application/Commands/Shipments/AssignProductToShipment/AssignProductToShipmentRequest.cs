using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MediatR;

namespace Warehouse.Application.Commands.Shipments.AssignProductToShipment;

public class AssignProductToShipmentRequest : IRequest<AssignProductToShipmentResponse>
{
    [JsonIgnore]
    public Guid ShipmentId { get; set; }

    public Guid ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Shipment product quantity must be greater than zero.")]
    public int Quantity { get; set; }
}