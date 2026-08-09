using AutoMapper;
using FluentAssertions;
using Moq;
using Warehouse.Api.UnitTests.Builders;
using Warehouse.Application.Queries.Products.GetExpiringSoonProducts;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Api.UnitTests.Queries.Products;

public class GetExpiringSoonProductsHandlerTests
{
    [Fact]
    public async Task Handle_ProductsExpireWithinThirtyDays_ReturnsProducts()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var mapperMock = new Mock<IMapper>();
        var timeProviderMock = new Mock<TimeProvider>();
        var fixedNow = new DateTimeOffset(2026, 8, 4, 15, 30, 0, TimeSpan.Zero);

        var startDate = fixedNow.UtcDateTime.Date;
        var endDateExclusive = startDate.AddDays(31);

        var products = new List<Product>
        {
            new ProductBuilder()
                .WithExpiryDate(startDate.AddDays(10))
                .Build()
        };

        var expectedResult = new List<ProductViewModel>
        {
            new()
            {
                Id = products[0].Id,
                Name = products[0].Name,
                SKU = products[0].SKU,
                ExpiryDate = products[0].ExpiryDate
            }
        };

        timeProviderMock
            .Setup(timeProvider => timeProvider.GetUtcNow())
            .Returns(fixedNow);

        productRepositoryMock
            .Setup(repository => repository.GetExpiringSoonAsync(startDate, endDateExclusive, CancellationToken.None))
            .ReturnsAsync(products);

        mapperMock
            .Setup(mapper => mapper.Map<List<ProductViewModel>>(products))
            .Returns(expectedResult);

        var handler = new GetExpiringSoonProductsHandler(productRepositoryMock.Object, mapperMock.Object, timeProviderMock.Object);

        var result = await handler.Handle(
            new GetExpiringSoonProductsRequest(),
            CancellationToken.None);

        result.Should().BeSameAs(expectedResult);

        productRepositoryMock.Verify(
            repository => repository.GetExpiringSoonAsync(startDate, endDateExclusive, CancellationToken.None),
            Times.Once);
    }
}