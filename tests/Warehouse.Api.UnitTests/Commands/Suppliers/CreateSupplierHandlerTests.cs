using AutoMapper;
using FluentAssertions;
using Moq;
using Warehouse.Api.UnitTests.Builders;
using Warehouse.Application.Commands.Suppliers.CreateSupplier;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Api.UnitTests.Commands.Suppliers;

public class CreateSupplierHandlerTests
{
    [Fact]
    public async Task Handle_ValidSupplier_CreatesSupplier()
    {
        var supplierRepositoryMock = new Mock<ISupplierRepository>();
        var mapperMock = new Mock<IMapper>();
        var supplierData = new SupplierBuilder().Build();

        var request = new CreateSupplierRequest
        {
            Name = supplierData.Name,
            Country = supplierData.Country,
            ContactEmail = supplierData.ContactEmail,
            PhoneNumber = supplierData.PhoneNumber
        };

        var expectedResult = new SupplierViewModel
        {
            Id = supplierData.SupplierId,
            Name = request.Name,
            Country = request.Country,
            ContactEmail = request.ContactEmail,
            PhoneNumber = request.PhoneNumber,
            IsActive = true
        };

        supplierRepositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<Supplier>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        mapperMock.Setup(mapper => mapper.Map<SupplierViewModel>(It.IsAny<Supplier>()))
            .Returns(expectedResult);

        var handler = new CreateSupplierHandler(supplierRepositoryMock.Object, mapperMock.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.Should().BeSameAs(expectedResult);

        supplierRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.Is<Supplier>(createdSupplier => createdSupplier.SupplierId != Guid.Empty &&
                                                   createdSupplier.Name == request.Name &&
                                                   createdSupplier.Country == request.Country &&
                                                   createdSupplier.ContactEmail == request.ContactEmail &&
                                                   createdSupplier.PhoneNumber == request.PhoneNumber &&
                                                   createdSupplier.IsActive),
                CancellationToken.None),
            Times.Once);
    }
}