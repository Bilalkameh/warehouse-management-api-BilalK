using AutoMapper;
using FluentAssertions;
using Moq;
using Warehouse.Api.UnitTests.Builders;
using Warehouse.Application.Queries.Products.SearchProducts;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;
using Warehouse.Application.Exceptions;


namespace Warehouse.Api.UnitTests.Queries.Products;

public class SearchProductsHandlerTests
{
    
    [Fact]
    public async Task Handle_SearchByName_ReturnsMatches()
    {
        
        var productRepositoryMock = new Mock<IProductRepository>();
        var mapperMock = new Mock<IMapper>();
        
        var product = new ProductBuilder()
            .WithName("keyboard")
            .Build();

        
        var request = new SearchProductsRequest
        {
            Name =product.Name
        };

        var products = new List<Product>
        {
            product
        };

        var expectedResult = new List<ProductViewModel>
        {
            new() { Name = product.Name,
                SKU = product.SKU }
        };

        productRepositoryMock
            .Setup(repository => repository.SearchAsync(request.Name, request.SupplierName, CancellationToken.None))
            .ReturnsAsync(products);

        mapperMock
            .Setup(mapper => mapper.Map<List<ProductViewModel>>(products))
            .Returns(expectedResult);

        var handler = new SearchProductsHandler(productRepositoryMock.Object, mapperMock.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        result.Should().BeSameAs(expectedResult);

        productRepositoryMock.Verify(repository => repository.SearchAsync(request.Name, request.SupplierName, CancellationToken.None),
            Times.Once);
    }
    
    
    [Fact]
    public async Task Handle_SearchByBothFilters_ReturnsIntersection()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var mapperMock = new Mock<IMapper>();

        var product = new ProductBuilder()
            .WithName("keyboard")
            .Build();

        var request = new SearchProductsRequest
        {
            Name = product.Name,
            SupplierName = product.Supplier.Name
        };

        var products = new List<Product>
        {
            product
        };

        var expectedResult = new List<ProductViewModel>
        {
            new() { Name = product.Name, SKU = product.SKU }
        };

        productRepositoryMock
            .Setup(repository => repository.SearchAsync(request.Name, request.SupplierName, CancellationToken.None))
            .ReturnsAsync(products);

        mapperMock.Setup(mapper => mapper.Map<List<ProductViewModel>>(products))
            .Returns(expectedResult);

        var handler = new SearchProductsHandler(productRepositoryMock.Object, mapperMock.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.Should().BeSameAs(expectedResult);

        productRepositoryMock.Verify(repository => repository.SearchAsync(request.Name, request.SupplierName, CancellationToken.None),
            Times.Once);
    }
    
    
    [Fact]
    public async Task Handle_SearchBySupplier_ReturnsMatches()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var mapperMock = new Mock<IMapper>();

        var product = new ProductBuilder().Build();
        var request = new SearchProductsRequest
        {
            SupplierName = product.Supplier.Name
        };

        var products = new List<Product>
        {
            product
        };

        var expectedResult = new List<ProductViewModel>
        {
            new() { Name = product.Name, SKU = product.SKU }
        };

        productRepositoryMock
            .Setup(repository => repository.SearchAsync(request.Name, request.SupplierName, CancellationToken.None))
            .ReturnsAsync(products);

        mapperMock
            .Setup(mapper => mapper.Map<List<ProductViewModel>>(products))
            .Returns(expectedResult);

        var handler = new SearchProductsHandler(productRepositoryMock.Object, mapperMock.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.Should().BeSameAs(expectedResult);

        productRepositoryMock.Verify(repository => repository.SearchAsync(request.Name, request.SupplierName, CancellationToken.None),
            Times.Once);
    }
    
    
    [Fact]
    public async Task Handle_EmptyFilters_ThrowsBadRequestException()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var mapperMock = new Mock<IMapper>();

        var request = new SearchProductsRequest();
        var handler = new SearchProductsHandler(productRepositoryMock.Object, mapperMock.Object);

        Func<Task> action = async () => await handler.Handle(request, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequestException>();

        productRepositoryMock.Verify(repository => repository.SearchAsync(request.Name, request.SupplierName, CancellationToken.None),
            Times.Never);
    }
}