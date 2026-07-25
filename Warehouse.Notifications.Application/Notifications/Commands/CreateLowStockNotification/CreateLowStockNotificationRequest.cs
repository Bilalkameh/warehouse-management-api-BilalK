using MediatR;

namespace Warehouse.Notifications.Application.Notifications.Commands.CreateLowStockNotification;

public record CreateLowStockNotificationRequest(
    Guid SourceEventId,
    DateTime EventTime,
    string CorrelationId,
    Guid ProductId,
    string ProductName,
    int CurrentQuantity,
    int Threshold) : IRequest<bool>;