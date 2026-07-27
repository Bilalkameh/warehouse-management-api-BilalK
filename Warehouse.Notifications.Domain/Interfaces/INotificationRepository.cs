using Warehouse.Notifications.Domain.Entities;
namespace Warehouse.Notifications.Domain.Interfaces;

public interface INotificationRepository
{
    Task<bool> ExistsBySourceEventIdAsync(Guid sourceEventId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Notification>> GetAllAsync(CancellationToken cancellationToken);

    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(Notification notification, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}