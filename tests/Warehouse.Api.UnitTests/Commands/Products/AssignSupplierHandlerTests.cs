using FluentAssertions;
using Moq;
using Warehouse.Api.UnitTests.Builders;
using Warehouse.Application.Commands.Products.AssignSupplier;
using Warehouse.Application.Exceptions;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Interfaces;
using Xunit;

namespace Warehouse.Api.UnitTests.Commands.Products;

public class AssignSupplierHandlerTests
{
    [Fact]
    public async Task Handle_ValidSupplier_AssignsSupplierToProduct()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var supplierRepositoryMock = new Mock<ISupplierRepository>();

        var product = new ProductBuilder().Build();
        var supplier = new SupplierBuilder().Build();

        var request = new AssignSupplierRequest
        {
            ProductId = product.Id,
            SupplierId = supplier.SupplierId
        };

        productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(request.ProductId, CancellationToken.None))
            .ReturnsAsync(product);

        supplierRepositoryMock
            .Setup(repository => repository.GetByIdAsync(request.SupplierId, CancellationToken.None))
            .ReturnsAsync(supplier);

        productRepositoryMock
            .Setup(repository => repository.UpdateAsync(product, CancellationToken.None))
            .Returns(Task.CompletedTask);

        var handler = new AssignSupplierHandler(productRepositoryMock.Object, supplierRepositoryMock.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeTrue();
        product.Supplier.Should().BeSameAs(supplier);
        product.SupplierId.Should().Be(supplier.SupplierId);

        productRepositoryMock.Verify(
            repository => repository.UpdateAsync(
                It.Is<Product>(updatedProduct => updatedProduct.Id == request.ProductId &&
                                                 updatedProduct.SupplierId == request.SupplierId &&
                                                 updatedProduct.Supplier == supplier),
                CancellationToken.None),
            Times.Once);
    }
    
    
    [Fact]
    public async Task Handle_ArchivedProduct_ThrowsBusinessRuleException()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var supplierRepositoryMock = new Mock<ISupplierRepository>();

        var product = new ProductBuilder().Build();
        var originalSupplier = product.Supplier;

        product.Archive();

        var supplier = new SupplierBuilder().Build();

        var request = new AssignSupplierRequest
        {
            ProductId = product.Id,
            SupplierId = supplier.SupplierId
        };

        productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(request.ProductId, CancellationToken.None))
            .ReturnsAsync(product);

        supplierRepositoryMock
            .Setup(repository => repository.GetByIdAsync(request.SupplierId, CancellationToken.None))
            .ReturnsAsync(supplier);

        var handler = new AssignSupplierHandler(productRepositoryMock.Object, supplierRepositoryMock.Object);

        Func<Task> action = async () => await handler.Handle(request, CancellationToken.None);

        await action.Should()
            .ThrowAsync<BusinessRuleException>();

        product.Supplier.Should().BeSameAs(originalSupplier);

        productRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<Product>(), CancellationToken.None),
            Times.Never);
    }
    
    
    [Fact]
    public async Task Handle_MissingSupplier_ThrowsNotFoundException()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var supplierRepositoryMock = new Mock<ISupplierRepository>();
        var product = new ProductBuilder().Build();

        var request = new AssignSupplierRequest
        {
            ProductId = product.Id,
            SupplierId = Guid.NewGuid()
        };

        productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(request.ProductId, CancellationToken.None))
            .ReturnsAsync(product);

        supplierRepositoryMock
            .Setup(repository => repository.GetByIdAsync(request.SupplierId, CancellationToken.None))
            .ReturnsAsync((Supplier?)null);

        var handler = new AssignSupplierHandler(productRepositoryMock.Object, supplierRepositoryMock.Object);

        Func<Task> action = async () => await handler.Handle(request, CancellationToken.None);

        await action.Should()
            .ThrowAsync<NotFoundException>();

        productRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<Product>(), CancellationToken.None),
            Times.Never);
    }
}