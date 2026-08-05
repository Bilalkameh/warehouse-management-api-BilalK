using FluentAssertions;
using Moq;
using Warehouse.Application.Commands.Shipments.UpdateShipmentStatus;
using Warehouse.Application.IntegrationEvents;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Enums;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Api.UnitTests.Commands.Shipments;

public class UpdateShipmentStatusHandlerTests
{
    [Fact]
    public async Task Handle_ValidStatusUpdate_PersistsAndPublishesNotification()
    {
        const string correlationId = "test-correlation-id";

        var supplier = new Supplier(
            "sup1",
            "Lebanon",
            "sup1@mail.com",
            "+961-1-000000");

        var shipment = new Shipment(
            "SHIP-001",
            supplier,
            "Beirut warehouse",
            DateTime.UtcNow.AddDays(5));

        var shipmentRepositoryMock = new Mock<IShipmentRepository>();
        var eventPublisherMock = new Mock<IWarehouseEventPublisher>();
        var correlationIdAccessorMock =
            new Mock<ICorrelationIdAccessor>();

        shipmentRepositoryMock
            .Setup(repository => repository.GetByIdAsync(
                shipment.Id,
                CancellationToken.None))
            .ReturnsAsync(shipment);

        correlationIdAccessorMock
            .Setup(accessor => accessor.CorrelationId)
            .Returns(correlationId);

        var handler = new UpdateShipmentStatusHandler(
            shipmentRepositoryMock.Object,
            eventPublisherMock.Object,
            correlationIdAccessorMock.Object);

        var request = new UpdateShipmentStatusRequest
        {
            ShipmentId = shipment.Id,
            Status = ShipmentStatus.InTransit
        };

        var result = await handler.Handle(
            request,
            CancellationToken.None);

        result.Success.Should().BeTrue();
        shipment.Status.Should().Be(ShipmentStatus.InTransit);

        shipmentRepositoryMock.Verify(
            repository => repository.UpdateAsync(
                shipment,
                CancellationToken.None),
            Times.Once);

        eventPublisherMock.Verify(
            publisher => publisher.PublishAsync(
                It.Is<ShipmentStatusUpdated>(warehouseEvent =>
                    warehouseEvent.RelatedEntityId == shipment.Id &&
                    warehouseEvent.SupplierId == supplier.SupplierId &&
                    warehouseEvent.SupplierEmail == supplier.ContactEmail &&
                    warehouseEvent.TrackingNumber == shipment.TrackingNumber &&
                    warehouseEvent.Status == "InTransit" &&
                    warehouseEvent.CorrelationId == correlationId),
                WarehouseEventRoutingKeys.ShipmentStatusUpdated,
                CancellationToken.None),
            Times.Once);
    }
}