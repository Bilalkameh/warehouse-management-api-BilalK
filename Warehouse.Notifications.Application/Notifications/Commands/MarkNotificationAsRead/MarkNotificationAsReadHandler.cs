using MediatR;
using Warehouse.Notifications.Domain.Interfaces;

namespace Warehouse.Notifications.Application.Notifications.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadHandler : IRequestHandler<MarkNotificationAsReadRequest, bool>
{
    private readonly INotificationRepository _repository;

    public MarkNotificationAsReadHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(MarkNotificationAsReadRequest request, CancellationToken cancellationToken)
    {
        var notification = await _repository.GetByIdAsync(request.NotificationId, cancellationToken);

        if (notification is null)
            return false;

        notification.MarkAsRead();

        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}