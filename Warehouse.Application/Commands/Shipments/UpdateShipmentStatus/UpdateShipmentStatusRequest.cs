using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MediatR;
using Warehouse.Domain.Enums;

namespace Warehouse.Application.Commands.Shipments.UpdateShipmentStatus;

public class UpdateShipmentStatusRequest : IRequest<UpdateShipmentStatusResponse>
{
    [JsonIgnore]
    public Guid ShipmentId { get; set; }

    [Required(ErrorMessage = "Shipment status is required.")]
    public ShipmentStatus? Status { get; set; }
}