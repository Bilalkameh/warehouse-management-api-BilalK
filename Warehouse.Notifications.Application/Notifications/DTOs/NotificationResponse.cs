using Warehouse.Notifications.Domain.Enums;

namespace Warehouse.Notifications.Application.Notifications.DTOs;

public class NotificationResponse
{
    public Guid Id { get; set; }
    public Guid SourceEventId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public Guid RelatedEntityId { get; set; }
    public string RelatedEntityType { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
}