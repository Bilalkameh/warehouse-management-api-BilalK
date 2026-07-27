using Microsoft.Extensions.Options;
using Warehouse.Application.IntegrationEvents;
using Warehouse.Application.Interfaces;
using Warehouse.Application.Settings;
using Warehouse.Domain.Entities;

namespace Warehouse.Application.Services;

public class LowStockEventService
{
    private readonly IWarehouseEventPublisher _eventPublisher;
    private readonly ICorrelationIdAccessor _correlationIdAccessor;
    private readonly LowStockSettings _settings;

    public LowStockEventService(IWarehouseEventPublisher eventPublisher, ICorrelationIdAccessor correlationIdAccessor, IOptions<LowStockSettings> lowStockOptions)
    {
        _eventPublisher = eventPublisher;
        _correlationIdAccessor = correlationIdAccessor;
        _settings = lowStockOptions.Value;
    }

    public async Task PublishIfStockBecameLowAsync( Product product, int previousQuantity, CancellationToken cancellationToken)
    {
        var crossedThreshold = previousQuantity >= _settings.Threshold && product.QuantityInStock < _settings.Threshold;

        if (!crossedThreshold)
            return;

        var stockLowEvent = new StockLowDetected
        {
            CorrelationId = _correlationIdAccessor.CorrelationId,
            RelatedEntityId = product.Id,
            ProductName = product.Name,
            CurrentQuantity = product.QuantityInStock,
            Threshold = _settings.Threshold
        };

        await _eventPublisher.PublishAsync(stockLowEvent, WarehouseEventRoutingKeys.StockLow, cancellationToken);
    }
}