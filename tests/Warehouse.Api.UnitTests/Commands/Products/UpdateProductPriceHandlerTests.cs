using FluentAssertions;
using Moq;
using Warehouse.Api.UnitTests.Builders;
using Xunit;
using Warehouse.Application.Commands.Products.UpdateProductPrice;
using Warehouse.Application.Interfaces;
using Warehouse.Application.Services;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Interfaces;


namespace Warehouse.Api.UnitTests.Commands.Products;

public class UpdateProductPriceHandlerTests
{
    [Fact]
    public async Task Handle_ValidPrice_UpdatesProductPrice()
    {
        var productRepositorymock = new Mock<IProductRepository>();
        var cacheMock = new Mock<ICacheService>(); 
        
        var product = new ProductBuilder()
            .WithPrice(10)
            .Build();

        var request = new UpdateProductPriceRequest
        {
            ProductId = product.Id,
            Price = 30 };
        
        productRepositorymock.Setup(repository => repository.GetByIdAsync(request.ProductId, CancellationToken.None))
            .ReturnsAsync(product);
        
        productRepositorymock.Setup(repository => repository.UpdateAsync(product, CancellationToken.None))
            .Returns(Task.CompletedTask);
        
        cacheMock.Setup(cache => cache.RemoveAsync(It.IsAny<string>(), CancellationToken.None))
            .Returns(Task.CompletedTask);
            
        var handler = new UpdateProductPriceHandler(productRepositorymock.Object, cacheMock.Object);
        
        var result = await handler.Handle(request, CancellationToken.None);
        
        result.Success.Should().BeTrue();
        product.Price.Should().Be(request.Price);
        
        productRepositorymock.Verify(repository => repository.UpdateAsync(It.Is<Product>(
                    updatedProduct => updatedProduct.Id == request.ProductId && updatedProduct.Price == request.Price),
                CancellationToken.None),
            Times.Once);
    }
    
    
    [Fact]
    public async Task Handle_InvalidPrice_ThrowsBusinessRuleException()
    {
        var productRepositorymock = new Mock<IProductRepository>();
        var cacheMock = new Mock<ICacheService>();
        
        var product = new ProductBuilder()
            .WithPrice(10)
            .Build();
        
        var originalPrice = product.Price;
        
        var request = new UpdateProductPriceRequest
        {
            ProductId = product.Id,
            Price = 0
        };
        
        productRepositorymock.Setup(repository => repository.GetByIdAsync(request.ProductId, CancellationToken.None))
            .ReturnsAsync(product); 
        
        var handler = new UpdateProductPriceHandler(productRepositorymock.Object, cacheMock.Object);
        
        // the point of using Func here is that we are expecting an exception, this allows fluentAssertions to verify the exception
        Func<Task> action = async () => await handler.Handle(request, CancellationToken.None);

        await action.Should()
            .ThrowAsync<BusinessRuleException>();
        
        product.Price.Should().Be(originalPrice);

        productRepositorymock.Verify(repository => repository.UpdateAsync(It.IsAny<Product>(), CancellationToken.None), Times.Never);
    }
}