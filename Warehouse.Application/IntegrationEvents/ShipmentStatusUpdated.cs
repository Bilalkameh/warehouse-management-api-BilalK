namespace Warehouse.Application.IntegrationEvents;

public class ShipmentStatusUpdated : WarehouseEvent
{
    public ShipmentStatusUpdated()
    {
        EventType = nameof(ShipmentStatusUpdated);
        RelatedEntityType = "Shipment";
        Severity = "Information";
    }

    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string SupplierEmail { get; set; } = string.Empty;
    public string TrackingNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}