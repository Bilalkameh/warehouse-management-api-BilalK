using MediatR;
using Warehouse.Notifications.Domain.Entities;
using Warehouse.Notifications.Domain.Interfaces;

namespace Warehouse.Notifications.Application.Notifications.Commands.CreateLowStockNotification;

public class CreateLowStockNotificationHandler : IRequestHandler<CreateLowStockNotificationRequest, bool>
{
    private readonly INotificationRepository _repository;

    public CreateLowStockNotificationHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(CreateLowStockNotificationRequest request, CancellationToken cancellationToken)
    {
        var alreadyProcessed = await _repository.ExistsBySourceEventIdAsync(request.SourceEventId, cancellationToken);

        if (alreadyProcessed)
            return false;

        var notification = Notification.CreateLowStock(
            request.SourceEventId,
            request.EventTime,
            request.CorrelationId,
            request.ProductId,
            request.ProductName,
            request.CurrentQuantity,
            request.Threshold);

        await _repository.AddAsync(notification, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}