using FluentAssertions;
using Moq;
using Warehouse.Api.UnitTests.Builders;
using Warehouse.Api.UnitTests.Helpers;
using Warehouse.Application.Commands.Products.AddProductImage;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;
using Xunit;

namespace Warehouse.Api.UnitTests.Commands.Products;

public class AddProductImageHandlerTests
{
    [Fact]
    public async Task Handle_ValidImage_GeneratesCorrectUploadPath()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var productImageRepositoryMock = new Mock<IProductImageRepository>();
        var fileStorageServiceMock = new Mock<IFileStorageService>();
        var product = new ProductBuilder().Build();

        var file = MultipartFormHelper.CreateFormFile("product.jpg", "image/jpeg");

        using var fileStream = file.OpenReadStream();

        var request = new AddProductImageRequest
        {
            ProductId = product.Id,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            FileStream = fileStream
        };

        string? uploadedObjectKey = null;

        productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(request.ProductId, CancellationToken.None))
            .ReturnsAsync(product);

        fileStorageServiceMock
            .Setup(service => service.UploadAsync(request.FileStream, request.FileSize, It.IsAny<string>(), 
                request.ContentType, CancellationToken.None))
            .Callback<Stream, long, string, string, CancellationToken>((_, _, objectKey, _, _) => uploadedObjectKey = objectKey)
            .Returns(Task.CompletedTask);

        productImageRepositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<ProductImage>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        var handler = new AddProductImageHandler(productRepositoryMock.Object, productImageRepositoryMock.Object, fileStorageServiceMock.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        uploadedObjectKey.Should().NotBeNull();

        var generatedPath = uploadedObjectKey!;

        generatedPath.Should().StartWith($"products/{product.Id}/");
        generatedPath.Should().EndWith(".jpg");

        Guid.TryParse(Path.GetFileNameWithoutExtension(generatedPath), out _)
            .Should()
            .BeTrue();

        result.Success.Should().BeTrue();
        result.Id.Should().NotBe(Guid.Empty);

        fileStorageServiceMock.Verify(
            service => service.UploadAsync(request.FileStream, request.FileSize, generatedPath, request.ContentType, CancellationToken.None),
            Times.Once);

        productImageRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.Is<ProductImage>(image => image.ProductId == product.Id &&
                                             image.FileName == request.FileName &&
                                             image.FilePath == generatedPath),
                CancellationToken.None),
            Times.Once);
    }
}