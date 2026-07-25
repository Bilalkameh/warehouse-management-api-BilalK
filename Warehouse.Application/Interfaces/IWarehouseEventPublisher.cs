using Warehouse.Application.IntegrationEvents;

namespace Warehouse.Application.Interfaces;

public interface IWarehouseEventPublisher
{
    Task PublishAsync<TEvent>(TEvent warehouseEvent, string routingKey, CancellationToken cancellationToken)
        where TEvent : WarehouseEvent;
}