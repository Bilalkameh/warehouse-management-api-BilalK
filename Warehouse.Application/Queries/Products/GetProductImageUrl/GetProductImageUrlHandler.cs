using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.GetProductImageUrl;

public class GetProductImageUrlHandler : IRequestHandler<GetProductImageUrlRequest, GetProductImageUrlResponse>
{
    private readonly IProductImageRepository _productImageRepository;
    private readonly IFileStorageService _fileStorageService;

    public GetProductImageUrlHandler(IProductImageRepository productImageRepository, IFileStorageService fileStorageService)
    {
        _productImageRepository = productImageRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<GetProductImageUrlResponse> Handle(GetProductImageUrlRequest request, CancellationToken cancellationToken)
    {
        var image = await _productImageRepository.GetByIdAsync(request.ImageId, cancellationToken);

        if (image == null)
            throw new NotFoundException("Product image was not found.");

        const int expirySeconds = 1000;

        var url = await _fileStorageService.GetPresignedUrlAsync(image.FilePath, expirySeconds);

        return new GetProductImageUrlResponse
        {
            Url = url,
            ExpiresInSeconds = expirySeconds
        };
    }
}