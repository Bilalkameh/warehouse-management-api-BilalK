namespace Warehouse.Notifications.Presentation.IntegrationEvents;

public class WarehouseFileUploaded : WarehouseEvent
{
    public Guid SupplierId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
}