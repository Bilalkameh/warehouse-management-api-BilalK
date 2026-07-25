using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Notifications.Domain.Interfaces;
using Warehouse.Notifications.Infrastructure.Persistence;

namespace Warehouse.Notifications.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<NotificationsDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<INotificationRepository, NotificationRepository>();
        return services;
    }
}