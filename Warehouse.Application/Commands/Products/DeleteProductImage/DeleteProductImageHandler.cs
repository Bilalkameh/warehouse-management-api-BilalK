using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.DeleteProductImage;

public class DeleteProductImageHandler : IRequestHandler<DeleteProductImageRequest>
{
    private readonly IProductImageRepository _productImageRepository;
    private readonly IFileStorageService _fileStorageService;

    public DeleteProductImageHandler(IProductImageRepository productImageRepository, IFileStorageService fileStorageService)
    {
        _productImageRepository = productImageRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task Handle(DeleteProductImageRequest request, CancellationToken cancellationToken)
    {
        var image = await _productImageRepository.GetByIdAsync(request.ImageId, cancellationToken);

        if (image == null)
            throw new NotFoundException(
                "Product image was not found.");

        await _fileStorageService.DeleteAsync(image.FilePath, cancellationToken);

        await _productImageRepository.DeleteAsync(image, cancellationToken);
    }
}