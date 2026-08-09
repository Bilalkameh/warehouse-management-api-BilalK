using System.ComponentModel.DataAnnotations;
using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Commands.Shipments.CreateShipment;

public class CreateShipmentRequest : IRequest<ShipmentViewModel>
{
    [Required(ErrorMessage = "Tracking number is required.")]
    [StringLength(100)]
    public string TrackingNumber { get; set; } = string.Empty;

    public Guid SupplierId { get; set; }

    [Required(ErrorMessage = "Destination address is required.")]
    [StringLength(250)]
    public string DestinationAddress { get; set; } = string.Empty;

    public DateTime EstimatedDeliveryDate { get; set; }
}