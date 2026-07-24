namespace Warehouse.Notifications.Api.IntegrationEvents;

public class WarehouseFileUploaded : WarehouseEvent
{
    public WarehouseFileUploaded()
    {
        EventType = nameof(WarehouseFileUploaded);
        RelatedEntityType = "SupplierDocument";
        Severity = "Information";
    }

    public Guid SupplierId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
}