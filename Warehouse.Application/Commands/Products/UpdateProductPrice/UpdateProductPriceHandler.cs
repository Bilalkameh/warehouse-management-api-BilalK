using MediatR;
using Warehouse.Application.Cache;
using Warehouse.Domain.Interfaces;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;


namespace Warehouse.Application.Commands.Products.UpdateProductPrice;

public class UpdateProductPriceHandler : IRequestHandler<UpdateProductPriceRequest, UpdateProductPriceResponse>
{
    private readonly IProductRepository _repository;
    private readonly ICacheService _cache;

    public UpdateProductPriceHandler(IProductRepository repository,  ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<UpdateProductPriceResponse> Handle(UpdateProductPriceRequest request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
            throw new NotFoundException("Product was not found.");

        product.UpdatePrice(request.Price);


        await _repository.UpdateAsync(product, cancellationToken);
        await _cache.RemoveAsync(ProductCacheKeys.ById(product.Id), cancellationToken);
        await _cache.RemoveAsync(ProductCacheKeys.All, cancellationToken);
        await _cache.RemoveAsync(ProductCacheKeys.Available, cancellationToken);

        return new UpdateProductPriceResponse
        {
            Success = true
        };
    }
}