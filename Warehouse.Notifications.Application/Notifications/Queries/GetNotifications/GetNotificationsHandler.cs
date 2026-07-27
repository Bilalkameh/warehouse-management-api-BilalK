using MediatR;
using Warehouse.Notifications.Application.Notifications.DTOs;
using Warehouse.Notifications.Domain.Interfaces;

namespace Warehouse.Notifications.Application.Notifications.Queries.GetNotifications;

public class GetNotificationsHandler : IRequestHandler<GetNotificationsRequest, IReadOnlyList<NotificationResponse>>
{
    private readonly INotificationRepository _repository;

    public GetNotificationsHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<NotificationResponse>> Handle(GetNotificationsRequest request, CancellationToken cancellationToken)
    {
        var notifications = await _repository.GetAllAsync(
            cancellationToken);

        return notifications.Select(notification => new NotificationResponse
            {
                Id = notification.Id,
                SourceEventId = notification.SourceEventId,
                Type = notification.Type,
                Title = notification.Title,
                Message = notification.Message,
                Severity = notification.Severity,
                Status = notification.Status,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt,
                RelatedEntityId = notification.RelatedEntityId,
                RelatedEntityType = notification.RelatedEntityType,
                CorrelationId = notification.CorrelationId
            })
            .ToList();
    }
}