using AutoMapper;
using FluentAssertions;
using Moq;
using Warehouse.Api.UnitTests.Builders;
using Warehouse.Application.Commands.Products.CreateProduct;
using Warehouse.Application.Interfaces;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;
using Warehouse.Application.Exceptions;
using Xunit;

namespace Warehouse.Api.UnitTests.Commands.Products;

public class CreateProductHandlerTests
{
    [Fact]
    public async Task Handle_ValidProduct_CreatesProduct()
    {
        //Arrange
        var productRepositoryMock = new Mock<IProductRepository>();
        var supplierRepositoryMock = new Mock<ISupplierRepository>();
        var mapperMock = new Mock<IMapper>();
        var cacheMock = new Mock<ICacheService>();

        var product = new ProductBuilder().Build();

        var request = new CreateProductRequest
        {
            Name = product.Name,
            SKU = product.SKU,
            Description = product.Description,
            Price = product.Price,
            QuantityInStock = product.QuantityInStock,
            SupplierName = product.Supplier.Name,
            ExpiryDate = product.ExpiryDate
        };
        
        var expectedResult = new ProductViewModel
        {
            Name = product.Name,
            SKU = product.SKU
        };

        supplierRepositoryMock
            .Setup(repository => repository.GetByNameAsync(request.SupplierName, CancellationToken.None))
            .ReturnsAsync(product.Supplier);

        productRepositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<Product>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        cacheMock
            .Setup(cache => cache.RemoveAsync(It.IsAny<string>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        mapperMock
            .Setup(mapper => mapper.Map<ProductViewModel>(It.IsAny<Product>()))
            .Returns(expectedResult);

        var handler = new CreateProductHandler(productRepositoryMock.Object, supplierRepositoryMock.Object, mapperMock.Object, cacheMock.Object);

        //Act 
        var result = await handler.Handle(request, CancellationToken.None);

        //Assert
        result.Should().BeSameAs(expectedResult);

        productRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.Is<Product>(createdProduct => createdProduct.Name == product.Name &&
                                                 createdProduct.SKU == product.SKU &&
                                                 createdProduct.Description == product.Description &&
                                                 createdProduct.Price == product.Price &&
                                                 createdProduct.QuantityInStock == product.QuantityInStock &&
                                                 createdProduct.Supplier == product.Supplier &&
                                                 createdProduct.ExpiryDate == product.ExpiryDate),
                CancellationToken.None),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_DuplicateSku_ThrowsConflictException()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var supplierRepositoryMock = new Mock<ISupplierRepository>();
        var mapperMock = new Mock<IMapper>();
        var cacheMock = new Mock<ICacheService>();

        var product = new ProductBuilder().Build();

        var request = new CreateProductRequest
        {
            Name = product.Name,
            SKU = product.SKU,
            Description = product.Description,
            Price = product.Price,
            QuantityInStock = product.QuantityInStock,
            SupplierName = product.Supplier.Name,
            ExpiryDate = product.ExpiryDate
        };

        productRepositoryMock
            .Setup(repository => repository.ExistsBySkuAsync(request.SKU, CancellationToken.None))
            .ReturnsAsync(true);

        supplierRepositoryMock
            .Setup(repository => repository.GetByNameAsync(request.SupplierName, CancellationToken.None))
            .ReturnsAsync(product.Supplier);

        var handler = new CreateProductHandler(productRepositoryMock.Object, supplierRepositoryMock.Object, mapperMock.Object, cacheMock.Object);
        
        Func<Task> action = async () => await handler.Handle(request, CancellationToken.None);
        
        await action.Should().ThrowAsync<ConflictException>();
    }
    [Fact]
    public async Task Handle_ValidProduct_SetsCreatedAt()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var supplierRepositoryMock = new Mock<ISupplierRepository>();
        var mapperMock = new Mock<IMapper>();
        var cacheMock = new Mock<ICacheService>();
        var product = new ProductBuilder().Build();

        var request = new CreateProductRequest
        {
         Name = product.Name,
         SKU = product.SKU,
         Description = product.Description,
         Price = product.Price,
         QuantityInStock = product.QuantityInStock,
         SupplierName = product.Supplier.Name,
         ExpiryDate = product.ExpiryDate
    };

    productRepositoryMock
        .Setup(repository => repository.ExistsBySkuAsync(request.SKU, CancellationToken.None))
        .ReturnsAsync(false);

    supplierRepositoryMock
        .Setup(repository => repository.GetByNameAsync(request.SupplierName, CancellationToken.None))
        .ReturnsAsync(product.Supplier);

    productRepositoryMock
        .Setup(repository => repository.AddAsync(It.IsAny<Product>(), CancellationToken.None))
        .Returns(Task.CompletedTask);

    cacheMock
        .Setup(cache => cache.RemoveAsync(It.IsAny<string>(), CancellationToken.None))
        .Returns(Task.CompletedTask);

    mapperMock
        .Setup(mapper => mapper.Map<ProductViewModel>(It.IsAny<Product>()))
        .Returns(new ProductViewModel());

    var handler = new CreateProductHandler(productRepositoryMock.Object, supplierRepositoryMock.Object, mapperMock.Object, cacheMock.Object);

    var beforeCreation = DateTime.UtcNow;

    await handler.Handle(request, CancellationToken.None);

    var afterCreation = DateTime.UtcNow;

    productRepositoryMock.Verify(
        repository => repository.AddAsync(
            It.Is<Product>(createdProduct =>
                createdProduct.CreatedAt >= beforeCreation &&
                createdProduct.CreatedAt <= afterCreation),
            CancellationToken.None),
        Times.Once);
}
    
    [Fact]
    public async Task Handle_ValidProduct_GeneratesId()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var supplierRepositoryMock = new Mock<ISupplierRepository>();
        var mapperMock = new Mock<IMapper>();
        var cacheMock = new Mock<ICacheService>();
        
        var product = new ProductBuilder().Build();
    
        var request = new CreateProductRequest
        {
            Name = product.Name,
            SKU = product.SKU,
            Description = product.Description,
            Price = product.Price,
            QuantityInStock = product.QuantityInStock,
            SupplierName = product.Supplier.Name,
            ExpiryDate = product.ExpiryDate
        };
    
        productRepositoryMock
            .Setup(repository => repository.ExistsBySkuAsync(request.SKU, CancellationToken.None))
            .ReturnsAsync(false);
    
        supplierRepositoryMock
            .Setup(repository => repository.GetByNameAsync(request.SupplierName, CancellationToken.None))
            .ReturnsAsync(product.Supplier);
    
        productRepositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<Product>(), CancellationToken.None))
            .Returns(Task.CompletedTask);
    
        cacheMock
            .Setup(cache => cache.RemoveAsync(It.IsAny<string>(), CancellationToken.None))
            .Returns(Task.CompletedTask);
    
        mapperMock
            .Setup(mapper => mapper.Map<ProductViewModel>(It.IsAny<Product>()))
            .Returns(new ProductViewModel());
    
        var handler = new CreateProductHandler(productRepositoryMock.Object, supplierRepositoryMock.Object, mapperMock.Object, cacheMock.Object);
    
        await handler.Handle(request, CancellationToken.None);
    
        productRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.Is<Product>(createdProduct => createdProduct.Id != Guid.Empty),
                CancellationToken.None),
            Times.Once);
    }


}
    