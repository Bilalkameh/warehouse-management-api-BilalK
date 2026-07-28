using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Warehouse.Api.UnitTests.Builders;
using Warehouse.Application.Commands.Products.UpdateProductQuantity;
using Warehouse.Application.Interfaces;
using Warehouse.Application.Services;
using Warehouse.Application.Settings;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;
using Warehouse.Domain.Exceptions;
using Xunit;

namespace Warehouse.Api.UnitTests.Commands.Products;

public class UpdateProductQuantityHandlerTests
{
    [Fact]
    public async Task Handle_ValidQuantity_UpdatesStock()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var cacheMock = new Mock<ICacheService>();
        var eventPublisherMock = new Mock<IWarehouseEventPublisher>();
        var correlationIdAccessorMock = new Mock<ICorrelationIdAccessor>();

        var lowStockOptions = Options.Create(new LowStockSettings 
            { Threshold = 10 });

        var lowStockEventService = new LowStockEventService(eventPublisherMock.Object, correlationIdAccessorMock.Object, lowStockOptions);

        var product = new ProductBuilder()
            .WithQuantity(25)
            .Build();

        var request = new UpdateProductQuantityRequest
        {
            ProductId = product.Id,
            Quantity = 40
        };

        productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(request.ProductId, CancellationToken.None))
            .ReturnsAsync(product);

        productRepositoryMock
            .Setup(repository => repository.UpdateAsync(product, CancellationToken.None))
            .Returns(Task.CompletedTask);

        cacheMock
            .Setup(cache => cache.RemoveAsync(It.IsAny<string>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        var handler = new UpdateProductQuantityHandler(productRepositoryMock.Object, cacheMock.Object, lowStockEventService);

        var result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeTrue();
        product.QuantityInStock.Should().Be(request.Quantity);

        productRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.Is<Product>(updatedProduct => updatedProduct.QuantityInStock == request.Quantity),
                CancellationToken.None),
            Times.Once);
    }
    
    
    [Fact]
    public async Task Handle_NegativeQuantity_ThrowsBusinessRuleException()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var cacheMock = new Mock<ICacheService>();
        var eventPublisherMock = new Mock<IWarehouseEventPublisher>();
        var correlationIdAccessorMock = new Mock<ICorrelationIdAccessor>();

        var lowStockOptions = Options.Create(new LowStockSettings
            { Threshold = 10 });

        var lowStockEventService = new LowStockEventService(eventPublisherMock.Object, correlationIdAccessorMock.Object, lowStockOptions);

        var product = new ProductBuilder()
            .WithQuantity(25)
            .Build();

        var originalQuantity = product.QuantityInStock;

        var request = new UpdateProductQuantityRequest
        {
            ProductId = product.Id,
            Quantity = -1
        };

        productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(request.ProductId, CancellationToken.None))
            .ReturnsAsync(product);

        var handler = new UpdateProductQuantityHandler(productRepositoryMock.Object, cacheMock.Object, lowStockEventService);

        Func<Task> action = async () => await handler.Handle(request, CancellationToken.None);

        await action.Should()
            .ThrowAsync<BusinessRuleException>()
            .WithMessage("Quantity cannot be negative.");

        product.QuantityInStock.Should().Be(originalQuantity);

        productRepositoryMock.Verify(repository => repository.UpdateAsync(It.IsAny<Product>(), CancellationToken.None),
            Times.Never);
    }
    
    
    
    [Fact]
    public async Task Handle_ValidQuantity_UpdatesLastUpdatedAt()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var cacheMock = new Mock<ICacheService>();
        var eventPublisherMock = new Mock<IWarehouseEventPublisher>();
        var correlationIdAccessorMock = new Mock<ICorrelationIdAccessor>();

        var lowStockOptions = Options.Create(new LowStockSettings
            { Threshold = 10 });

        var lowStockEventService = new LowStockEventService(eventPublisherMock.Object, correlationIdAccessorMock.Object, lowStockOptions);

        var product = new ProductBuilder()
            .WithQuantity(25)
            .Build();

        var previousLastUpdatedAt = product.LastUpdatedAt;

        var request = new UpdateProductQuantityRequest
        {
            ProductId = product.Id,
            Quantity = 40
        };

        productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(request.ProductId, CancellationToken.None))
            .ReturnsAsync(product);

        productRepositoryMock
            .Setup(repository => repository.UpdateAsync(product, CancellationToken.None))
            .Returns(Task.CompletedTask);

        cacheMock
            .Setup(cache => cache.RemoveAsync(It.IsAny<string>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        var handler = new UpdateProductQuantityHandler(productRepositoryMock.Object, cacheMock.Object, lowStockEventService);
        await Task.Delay(10);
        var result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeTrue();
        product.LastUpdatedAt.Should().BeAfter(previousLastUpdatedAt);
    }

}