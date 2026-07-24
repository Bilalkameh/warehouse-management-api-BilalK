namespace Warehouse.Notifications.Api.IntegrationEvents;

public class WarehouseEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime EventTime { get; set; } = DateTime.UtcNow;
    public string CorrelationId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public Guid RelatedEntityId { get; set; }
    public string RelatedEntityType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
}