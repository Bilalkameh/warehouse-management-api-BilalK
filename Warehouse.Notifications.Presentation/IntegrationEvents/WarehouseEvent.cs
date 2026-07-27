namespace Warehouse.Notifications.Presentation.IntegrationEvents;

public class WarehouseEvent
{
    public Guid EventId { get; set; }
    public DateTime EventTime { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public Guid RelatedEntityId { get; set; }
    public string RelatedEntityType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
}