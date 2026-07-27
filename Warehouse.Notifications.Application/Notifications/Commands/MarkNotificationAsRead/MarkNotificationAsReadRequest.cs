using MediatR;

namespace Warehouse.Notifications.Application.Notifications.Commands.MarkNotificationAsRead;

public record MarkNotificationAsReadRequest(Guid NotificationId) : IRequest<bool>;