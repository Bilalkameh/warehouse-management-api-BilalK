using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Application.IntegrationEvents;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Shipments.UpdateShipmentStatus;

public class UpdateShipmentStatusHandler
    : IRequestHandler<UpdateShipmentStatusRequest, UpdateShipmentStatusResponse>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IWarehouseEventPublisher _eventPublisher;
    private readonly ICorrelationIdAccessor _correlationIdAccessor;

    public UpdateShipmentStatusHandler(IShipmentRepository shipmentRepository, IWarehouseEventPublisher eventPublisher, ICorrelationIdAccessor correlationIdAccessor)
    {
        _shipmentRepository = shipmentRepository;
        _eventPublisher = eventPublisher;
        _correlationIdAccessor = correlationIdAccessor;
    }

    public async Task<UpdateShipmentStatusResponse> Handle(UpdateShipmentStatusRequest request, CancellationToken cancellationToken)
    {
        if (request.Status == null)
        {
            throw new BadRequestException("Shipment status is required.");
        }

        var shipment = await _shipmentRepository.GetByIdAsync(request.ShipmentId, cancellationToken)
            ?? throw new NotFoundException("Shipment was not found.");

        if (shipment.Status == request.Status.Value)
        {
            return new UpdateShipmentStatusResponse
            {
                Success = true
            };
        }

        shipment.UpdateStatus(request.Status.Value);

        await _shipmentRepository.UpdateAsync(shipment, cancellationToken);

        var shipmentStatusUpdated = new ShipmentStatusUpdated
        {
            CorrelationId = _correlationIdAccessor.CorrelationId,
            RelatedEntityId = shipment.Id,
            SupplierId = shipment.SupplierId,
            SupplierName = shipment.Supplier.Name,
            SupplierEmail = shipment.Supplier.ContactEmail,
            TrackingNumber = shipment.TrackingNumber,
            Status = shipment.Status.ToString()
        };

        await _eventPublisher.PublishAsync(shipmentStatusUpdated, WarehouseEventRoutingKeys.ShipmentStatusUpdated, cancellationToken);

        return new UpdateShipmentStatusResponse
        {
            Success = true
        };
    }
}