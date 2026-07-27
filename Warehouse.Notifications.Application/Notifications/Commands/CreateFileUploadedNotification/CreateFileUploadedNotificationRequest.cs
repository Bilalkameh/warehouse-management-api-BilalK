using MediatR;

namespace Warehouse.Notifications.Application.Notifications.Commands.CreateFileUploadedNotification;

public record CreateFileUploadedNotificationRequest(
    Guid SourceEventId,
    DateTime EventTime,
    string CorrelationId,
    Guid RelatedEntityId,
    Guid SupplierId,
    string FileName) : IRequest<bool>;