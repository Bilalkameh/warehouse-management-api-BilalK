namespace Warehouse.Notifications.Presentation.IntegrationEvents;

public class StockLowDetected : WarehouseEvent
{
    public string ProductName { get; set; } = string.Empty;
    public int CurrentQuantity { get; set; }
    public int Threshold { get; set; }
}