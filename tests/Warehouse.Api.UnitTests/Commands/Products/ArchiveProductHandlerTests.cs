using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Warehouse.Api.UnitTests.Builders;
using Warehouse.Application.Cache;
using Warehouse.Application.Commands.Products.ArchiveProduct;
using Warehouse.Application.Interfaces;
using Warehouse.Application.Queries.Products.GetProducts;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Api.UnitTests.Commands.Products;

public class ArchiveProductHandlerTests
{
    [Fact]
    public async Task Handle_ExistingProduct_MarksProductArchived()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var cacheMock = new Mock<ICacheService>();
        var product = new ProductBuilder().Build();

        var request = new ArchiveProductRequest
        {
            ProductId = product.Id
        };

        productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(request.ProductId, CancellationToken.None))
            .ReturnsAsync(product);

        productRepositoryMock.Setup(repository => repository.UpdateAsync(product, CancellationToken.None))
            .Returns(Task.CompletedTask);

        cacheMock.Setup(cache => cache.RemoveAsync(It.IsAny<string>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        var handler = new ArchiveProductHandler(productRepositoryMock.Object, cacheMock.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeTrue();
        product.IsArchived.Should().BeTrue();

        productRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.Is<Product>(updatedProduct => 
                    updatedProduct.Id == request.ProductId && updatedProduct.IsArchived), 
                CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ArchivedProduct_RemainsInList()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<ILogger<GetProductsHandler>>();
        var cacheMock = new Mock<ICacheService>();

        var product = new ProductBuilder().Build();
        product.Archive();

        var request = new GetProductsRequest
        {
            OnlyAvailable = false
        };

        var products = new List<Product>
        {
            product
        };

        var expectedResult = new List<ProductViewModel>
        {
            new()
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                IsArchived = true
            }
        };

        cacheMock
            .Setup(cache => cache.GetAsync(ProductCacheKeys.All, CancellationToken.None))
            .ReturnsAsync((string?)null);

        productRepositoryMock.Setup(repository => repository.GetAllAsync(CancellationToken.None))
            .ReturnsAsync(products);

        mapperMock.Setup(mapper => mapper.Map<List<ProductViewModel>>(products))
            .Returns(expectedResult);

        cacheMock
            .Setup(cache => cache.SetAsync(ProductCacheKeys.All, It.IsAny<string>(), It.IsAny<TimeSpan>(), 
                CancellationToken.None))
            .Returns(Task.CompletedTask);

        var handler = new GetProductsHandler(productRepositoryMock.Object, mapperMock.Object, loggerMock.Object, cacheMock.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.Should().ContainSingle(returnedProduct => 
            returnedProduct.Id == product.Id && returnedProduct.IsArchived);

        productRepositoryMock.Verify(repository => repository.GetAllAsync(CancellationToken.None), Times.Once);

        productRepositoryMock.Verify(repository => repository.GetAvailableAsync(CancellationToken.None), Times.Never);
    }
}