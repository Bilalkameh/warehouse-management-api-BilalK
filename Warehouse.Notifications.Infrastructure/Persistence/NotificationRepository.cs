using Microsoft.EntityFrameworkCore;
using Warehouse.Notifications.Domain.Entities;
using Warehouse.Notifications.Domain.Interfaces;

namespace Warehouse.Notifications.Infrastructure.Persistence;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationsDbContext _context;

    public NotificationRepository(NotificationsDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsBySourceEventIdAsync(Guid sourceEventId, CancellationToken cancellationToken)
    {
        return _context.Notifications.AnyAsync(notification => notification.SourceEventId == sourceEventId, cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Notifications
            .AsNoTracking()
            .OrderByDescending(notification => notification.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Notifications.FirstOrDefaultAsync(notification => notification.Id == id, cancellationToken);
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken)
    {
        await _context.Notifications.AddAsync(notification, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}