namespace Warehouse.Application.IntegrationEvents;

public class StockLowDetected : WarehouseEvent
{
    public StockLowDetected()
    {
        EventType = nameof(StockLowDetected);
        RelatedEntityType = "Product";
        Severity = "Warning";
    }

    public string ProductName { get; set; } = string.Empty;
    public int CurrentQuantity { get; set; }
    public int Threshold { get; set; }
}