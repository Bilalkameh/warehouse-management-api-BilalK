using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;
using Warehouse.Application.Interfaces;

namespace Warehouse.Application.Commands.Products.AddProductImage;

public class AddProductImageHandler : IRequestHandler<AddProductImageRequest, AddProductImageResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductImageRepository _productImageRepository;
    private readonly IFileStorageService _fileStorageService;

    public AddProductImageHandler(IProductRepository productRepository, IProductImageRepository productImageRepository,  IFileStorageService fileStorageService)
    {
        _productRepository = productRepository;
        _productImageRepository = productImageRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<AddProductImageResponse> Handle(AddProductImageRequest request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
            throw new NotFoundException("Product was not found.");
        
        var extension = Path.GetExtension(request.FileName);

        var objectKey = $"products/{product.Id}/{Guid.NewGuid()}{extension}";
        
        await _fileStorageService.UploadAsync(request.FileStream, request.FileSize, objectKey, request.ContentType, cancellationToken);
        
        var image = new ProductImage(product, request.FileName, objectKey);
        
        await _productImageRepository.AddAsync(image, cancellationToken);

        return new AddProductImageResponse
        {
            Id = image.Id,
            Success = true,
        };
    }
}