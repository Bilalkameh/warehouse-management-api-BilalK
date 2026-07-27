using Microsoft.EntityFrameworkCore;
using Warehouse.Notifications.Domain.Entities;

namespace Warehouse.Notifications.Infrastructure.Persistence;

public class NotificationsDbContext : DbContext
{
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : base(options)
    {
    }

    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Notification>()
            .HasIndex(notification => notification.SourceEventId)
            .IsUnique();

        modelBuilder.Entity<Notification>()
            .Property(notification => notification.Status)
            .HasConversion<string>();
    }
}