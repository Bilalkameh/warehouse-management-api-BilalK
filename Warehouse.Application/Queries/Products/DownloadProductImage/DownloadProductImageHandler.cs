using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.DownloadProductImage;

public class DownloadProductImageHandler : IRequestHandler<DownloadProductImageRequest, DownloadProductImageResponse>
{
    private readonly IProductImageRepository _productImageRepository;
    private readonly IFileStorageService _fileStorageService;

    public DownloadProductImageHandler(IProductImageRepository productImageRepository, IFileStorageService fileStorageService)
    {
        _productImageRepository = productImageRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<DownloadProductImageResponse> Handle(DownloadProductImageRequest request, CancellationToken cancellationToken)
    {
        var image = await _productImageRepository.GetByIdAsync(request.ImageId, cancellationToken);

        if (image == null)
            throw new NotFoundException(
                "Product image was not found.");

        var fileData = await _fileStorageService.DownloadAsync(image.FilePath, cancellationToken);

        return new DownloadProductImageResponse
        {
            FileName = image.FileName,
            FileData = fileData
        };
    }
}