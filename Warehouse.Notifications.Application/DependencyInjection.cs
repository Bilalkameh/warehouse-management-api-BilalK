using Microsoft.Extensions.DependencyInjection;
using Warehouse.Notifications.Application.Notifications.Queries.GetNotifications;

namespace Warehouse.Notifications.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblyContaining<GetNotificationsRequest>();
        });

        return services;
    }
}