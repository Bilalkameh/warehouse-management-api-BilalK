using AutoMapper;
using FluentAssertions;
using Moq;
using Warehouse.Application.Commands.Shipments.CreateShipment;
using Warehouse.Application.Exceptions;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Api.UnitTests.Commands.Shipments;

public class CreateShipmentHandlerTests
{
    [Fact]
    public async Task Handle_DuplicateTrackingNumber_ThrowsConflictException()
    {
        var shipmentRepositoryMock = new Mock<IShipmentRepository>();
        var supplierRepositoryMock = new Mock<ISupplierRepository>();
        var mapperMock = new Mock<IMapper>();

        var request = new CreateShipmentRequest
        {
            TrackingNumber = "SHIP-001",
            SupplierId = Guid.NewGuid(),
            DestinationAddress = "Beirut warehouse",
            EstimatedDeliveryDate = DateTime.UtcNow.AddDays(5)
        };

        shipmentRepositoryMock
            .Setup(repository => repository.ExistsByTrackingNumberAsync(
                request.TrackingNumber,
                CancellationToken.None))
            .ReturnsAsync(true);

        var handler = new CreateShipmentHandler(
            shipmentRepositoryMock.Object,
            supplierRepositoryMock.Object,
            mapperMock.Object);

        Func<Task> action = async () =>
            await handler.Handle(request, CancellationToken.None);

        await action.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage(
                "A shipment with this tracking number already exists.");

        supplierRepositoryMock.Verify(
            repository => repository.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        shipmentRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.IsAny<Warehouse.Domain.Entities.Shipment>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}