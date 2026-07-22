using MediatR;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.GetProductImages;

public class GetProductImagesHandler : IRequestHandler<GetProductImagesRequest, List<GetProductImagesResponse>>
{
    private readonly IProductImageRepository _productImageRepository;

    public GetProductImagesHandler(IProductImageRepository productImageRepository)
    {
        _productImageRepository = productImageRepository;
    }

    public async Task<List<GetProductImagesResponse>> Handle(GetProductImagesRequest request, CancellationToken cancellationToken)
    {
        var images = await _productImageRepository.GetByProductIdAsync(request.ProductId, cancellationToken);

        return images
            .Select(image => new GetProductImagesResponse
            {
                Id = image.Id,
                FileName = image.FileName
            })
            .ToList();
    }
}