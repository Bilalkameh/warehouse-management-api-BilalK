using FluentAssertions;
using Moq;
using Warehouse.Api.UnitTests.Builders;
using Warehouse.Application.Commands.Suppliers.DeactivateSupplier;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;
using Xunit;

namespace Warehouse.Api.UnitTests.Commands.Suppliers;

public class DeactivateSupplierHandlerTests
{
    [Fact]
    public async Task Handle_ExistingSupplier_DeactivatesSupplier()
    {
        var supplierRepositoryMock = new Mock<ISupplierRepository>();

        var supplier = new SupplierBuilder().Build();

        var request = new DeactivateSupplierRequest
        {
            SupplierId = supplier.SupplierId
        };

        supplierRepositoryMock
            .Setup(repository => repository.GetByIdAsync(request.SupplierId, CancellationToken.None))
            .ReturnsAsync(supplier);

        supplierRepositoryMock
            .Setup(repository => repository.UpdateAsync(supplier, CancellationToken.None))
            .Returns(Task.CompletedTask);

        var handler = new DeactivateSupplierHandler(supplierRepositoryMock.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeTrue();
        supplier.IsActive.Should().BeFalse();

        supplierRepositoryMock.Verify(
            repository => repository.UpdateAsync(
                It.Is<Supplier>(updatedSupplier => updatedSupplier.SupplierId == request.SupplierId && !updatedSupplier.IsActive),
                CancellationToken.None),
            Times.Once);
    }
}