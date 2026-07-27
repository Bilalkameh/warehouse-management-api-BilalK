using Warehouse.Notifications.Domain.Enums;

namespace Warehouse.Notifications.Domain.Entities;

public class Notification
{


    private Notification(Guid sourceEventId, string type, string title, string message, string severity,
        DateTime createdAt, Guid relatedEntityId, string relatedEntityType, string correlationId)
    {
        Id = Guid.NewGuid();
        SourceEventId = sourceEventId;
        Type = type;
        Title = title;
        Message = message;
        Severity = severity;
        Status = NotificationStatus.Unread;
        CreatedAt = createdAt;
        RelatedEntityId = relatedEntityId;
        RelatedEntityType = relatedEntityType;
        CorrelationId = correlationId;
    }
    
    private Notification()
    {
    }

    public Guid Id { get; private set; }
    public Guid SourceEventId { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string Severity { get; private set; } = string.Empty;
    public NotificationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public Guid RelatedEntityId { get; private set; }
    public string RelatedEntityType { get; private set; } = string.Empty;
    public string CorrelationId { get; private set; } = string.Empty;

    public static Notification CreateLowStock(Guid sourceEventId, DateTime eventTime, string correlationId, 
        Guid productId, string productName, int currentQuantity, int threshold)
    {
        ValidateCommonData(sourceEventId, eventTime, correlationId, productId);

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name is required.");

        if (currentQuantity < 0)
            throw new ArgumentException("Current quantity cannot be negative.");

        if (threshold <= 0)
            throw new ArgumentException("Low-stock threshold must be greater than zero.");

        if (currentQuantity >= threshold)
            throw new InvalidOperationException("A low-stock notification requires a quantity below the threshold.");

        return new Notification(
            sourceEventId,
            type: "StockLowDetected",
            title: "Low stock detected",
            message: $"{productName} now has {currentQuantity} units. " + $"The low-stock threshold is {threshold}.",
            severity: "Warning",
            createdAt: eventTime,
            relatedEntityId: productId,
            relatedEntityType: "Product",
            correlationId: correlationId);
    }

    public static Notification CreateFileUploaded(Guid sourceEventId, DateTime eventTime, 
        string correlationId, Guid relatedEntityId, Guid supplierId, string fileName)
    {
        ValidateCommonData(sourceEventId, eventTime, correlationId, relatedEntityId);

        if (supplierId == Guid.Empty)
            throw new ArgumentException("Supplier ID is required.");

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required.");

        return new Notification(sourceEventId,
            type: "WarehouseFileUploaded",
            title: "Supplier document uploaded",
            message: $"{fileName} was uploaded for supplier {supplierId}.",
            severity: "Information",
            createdAt: eventTime,
            relatedEntityId: relatedEntityId,
            relatedEntityType: "SupplierDocument",
            correlationId: correlationId);
    }

    public void MarkAsRead()
    {
        if (Status == NotificationStatus.Read)
            return;
        Status = NotificationStatus.Read;
        ReadAt = DateTime.UtcNow;
    }

    private static void ValidateCommonData(Guid sourceEventId, DateTime eventTime, string correlationId, Guid relatedEntityId)
    {
        if (sourceEventId == Guid.Empty)
            throw new ArgumentException("Source event ID is required.");

        if (eventTime == default)
            throw new ArgumentException("Event time is required.");

        if (string.IsNullOrWhiteSpace(correlationId))
            throw new ArgumentException("Correlation ID is required.");

        if (relatedEntityId == Guid.Empty)
            throw new ArgumentException("Related entity ID is required.");
    }
}