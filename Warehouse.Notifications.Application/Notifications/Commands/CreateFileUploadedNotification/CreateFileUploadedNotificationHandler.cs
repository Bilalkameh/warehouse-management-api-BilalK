using MediatR;
using Warehouse.Notifications.Domain.Entities;
using Warehouse.Notifications.Domain.Interfaces;

namespace Warehouse.Notifications.Application.Notifications.Commands.CreateFileUploadedNotification;

public class CreateFileUploadedNotificationHandler : IRequestHandler<CreateFileUploadedNotificationRequest, bool>
{
    
    private readonly INotificationRepository _repository;

    public CreateFileUploadedNotificationHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(CreateFileUploadedNotificationRequest request, CancellationToken cancellationToken)
    {
        var alreadyProcessed = await _repository.ExistsBySourceEventIdAsync(request.SourceEventId, cancellationToken);

        if (alreadyProcessed)
            return false;

        var notification = Notification.CreateFileUploaded(
            request.SourceEventId,
            request.EventTime,
            request.CorrelationId,
            request.RelatedEntityId,
            request.SupplierId,
            request.FileName);

        await _repository.AddAsync(notification, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}