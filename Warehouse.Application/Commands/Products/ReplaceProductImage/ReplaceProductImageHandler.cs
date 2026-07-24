using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.ReplaceProductImage;

public class ReplaceProductImageHandler : IRequestHandler<ReplaceProductImageRequest, ReplaceProductImageResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductImageRepository _productImageRepository;
    private readonly IFileStorageService _fileStorageService;

    public ReplaceProductImageHandler(IProductRepository productRepository, IProductImageRepository productImageRepository, IFileStorageService fileStorageService)
    {
        _productRepository = productRepository;
        _productImageRepository = productImageRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<ReplaceProductImageResponse> Handle(ReplaceProductImageRequest request, CancellationToken cancellationToken)
    {
        var oldImage = await _productImageRepository.GetByIdAsync(request.ImageId, cancellationToken);

        if (oldImage == null)
            throw new NotFoundException("Product image was not found.");

        var product = await _productRepository.GetByIdAsync(oldImage.ProductId, cancellationToken);

        if (product == null)
            throw new NotFoundException("Product was not found.");

        var extension = Path.GetExtension(request.FileName);

        var newFilePath = $"products/{product.Id}/{Guid.NewGuid()}{extension}";

        await _fileStorageService.UploadAsync(request.FileStream, request.FileSize, newFilePath, request.ContentType, cancellationToken);

        var newImage = new ProductImage(product, request.FileName, newFilePath);

        await _productImageRepository.ReplaceAsync(oldImage, newImage, cancellationToken);

        await _fileStorageService.DeleteAsync(oldImage.FilePath, cancellationToken);

        return new ReplaceProductImageResponse
        {
            Id = newImage.Id,
            Success = true
        };
    }
}