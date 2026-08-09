using Warehouse.Domain.Enums;

namespace Warehouse.Application.ViewModels;

public class ShipmentViewModel
{
    public Guid Id { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string DestinationAddress { get; set; } = string.Empty;
    public DateTime EstimatedDeliveryDate { get; set; }
    public ShipmentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public List<ShipmentProductViewModel> Products { get; set; } = new();
}