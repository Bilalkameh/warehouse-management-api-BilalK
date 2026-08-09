using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Warehouse.Api.UnitTests.Helpers;
using Warehouse.Application.Commands.Products.AddProductImage;
using Warehouse.Presentation.Controllers;
using Warehouse.Application.Queries.Products.GetExpiringSoonProducts;
using Warehouse.Application.ViewModels;

namespace Warehouse.Api.UnitTests.Controllers;

public class ProductsControllerTests
{
    [Theory]
    [InlineData("product.jpg", "image/jpeg")]
    [InlineData("product.png", "image/png")]
    public async Task UploadImage_ValidImage_ReturnsOk(string fileName, string contentType)
    {
        var mediatorMock = new Mock<IMediator>();
        var productId = Guid.NewGuid();

        var file = MultipartFormHelper.CreateFormFile(fileName, contentType);

        var expectedResult = new AddProductImageResponse
        {
            Id = Guid.NewGuid(),
            Success = true
        };

        mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<AddProductImageRequest>(), CancellationToken.None))
            .ReturnsAsync(expectedResult);

        var controller = new ProductsController(mediatorMock.Object);

        var result = await controller.UploadImage(productId, file, CancellationToken.None);

        var okResult = result.Should()
            .BeOfType<OkObjectResult>()
            .Which;

        okResult.Value.Should().BeSameAs(expectedResult);

        mediatorMock.Verify(
            mediator => mediator.Send(
                It.Is<AddProductImageRequest>(request => request.ProductId == productId &&
                                                         request.FileName == fileName &&
                                                         request.ContentType == contentType &&
                                                         request.FileSize == file.Length),
                CancellationToken.None),
            Times.Once);
    }


    [Fact]
    public async Task UploadImage_InvalidExtension_ReturnsBadRequest()
    {
        var mediatorMock = new Mock<IMediator>();
        var file = MultipartFormHelper.CreateFormFile("product.txt", "text/plain");
        var controller = new ProductsController(mediatorMock.Object);
        var result = await controller.UploadImage(Guid.NewGuid(), file, CancellationToken.None);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Which;

        badRequest.Value.Should()
            .Be("Only Jpeg and Png images are allowed.");

        mediatorMock.Verify(mediator => mediator.Send(It.IsAny<AddProductImageRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }


    [Fact]
    public async Task UploadImage_FileLargerThan5MB_ReturnsBadRequest()
    {
        var mediatorMock = new Mock<IMediator>();
        const int maxFileSize = 5 * 1024 * 1024;
        var file = MultipartFormHelper.CreateFormFile("product.jpg", "image/jpeg", maxFileSize + 1);
        var controller = new ProductsController(mediatorMock.Object);

        var result = await controller.UploadImage(Guid.NewGuid(), file, CancellationToken.None);

        var badRequest = result.Should()
            .BeOfType<BadRequestObjectResult>().Which;

        badRequest.Value.Should().Be("Image cannot exceed 5 MB.");

        mediatorMock.Verify(mediator => mediator.Send(It.IsAny<AddProductImageRequest>(),
                It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task GetExpiringSoonProducts_ReturnsOk()
    {
        var mediatorMock = new Mock<IMediator>();

        var expectedResult = new List<ProductViewModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Milk",
                SKU = "MILK-001",
                ExpiryDate = new DateTime(2026, 8, 9)
            }
        };

        mediatorMock
            .Setup(mediator => mediator.Send(
                It.IsAny<GetExpiringSoonProductsRequest>(),
                CancellationToken.None))
            .ReturnsAsync(expectedResult);

        var controller = new ProductsController(mediatorMock.Object);

        var result = await controller.GetExpiringSoonProducts(
            CancellationToken.None);

        var okResult = result.Should()
            .BeOfType<OkObjectResult>()
            .Which;

        okResult.Value.Should().BeSameAs(expectedResult);

        mediatorMock.Verify(
            mediator => mediator.Send(
                It.IsAny<GetExpiringSoonProductsRequest>(),
                CancellationToken.None),
            Times.Once);
    }
}