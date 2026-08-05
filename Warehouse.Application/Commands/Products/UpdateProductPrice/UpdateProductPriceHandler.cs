using MediatR;
using Warehouse.Application.Cache;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.UpdateProductPrice;

public class UpdateProductPriceHandler
    : IRequestHandler<UpdateProductPriceRequest, UpdateProductPriceResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cache;

    public UpdateProductPriceHandler(IProductRepository productRepository, ICacheService cache)
    {
        _productRepository = productRepository;
        _cache = cache;
    }

    public async Task<UpdateProductPriceResponse> Handle(UpdateProductPriceRequest request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
                      ?? throw new NotFoundException("Product was not found.");

        product.UpdatePrice(request.Price);

        await _productRepository.UpdateAsync(product, cancellationToken);

        await ProductCacheInvalidator.InvalidateProductAsync(_cache, product.Id, cancellationToken);

        return new UpdateProductPriceResponse
        {
            Success = true
        };
    }
}