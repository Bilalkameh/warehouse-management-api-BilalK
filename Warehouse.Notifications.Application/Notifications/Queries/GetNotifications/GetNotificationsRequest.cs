using MediatR;
using Warehouse.Notifications.Application.Notifications.DTOs;

namespace Warehouse.Notifications.Application.Notifications.Queries.GetNotifications;

public record GetNotificationsRequest() : IRequest<IReadOnlyList<NotificationResponse>>;